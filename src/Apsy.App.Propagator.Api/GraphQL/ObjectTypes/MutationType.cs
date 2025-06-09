using Aps.CommonBack.Base.Attributes;
using Aps.CommonBack.Base.Enums;
using Aps.CommonBack.Base.Extensions;
using Apsy.App.Propagator.Api.GraphQL.Mutations;
using Humanizer;
using System.Reflection;

namespace Propagator.Api.GraphQL.ObjectTypes;

public class MutationType : ObjectType<Mutation>
{
    protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
    {
        //var allMutations = AppDomain.CurrentDomain.GetAssemblies()
        //           .SelectMany(t => t.GetTypes())
        //           .Where(t => t.IsClass && t.Namespace == typeof(Mutation).Namespace);
        var allMutations = AppDomain.CurrentDomain.GetAssemblies()
              .Where(a => !a.IsDynamic) // Skip dynamic assemblies
              .SelectMany(a =>
              {
                  try
                  {
                      return a.GetTypes();
                  }
                  catch (ReflectionTypeLoadException ex)
                  {
                      return ex.Types?.Where(t => t != null) ?? Enumerable.Empty<Type>();
                  }
                  catch
                  {
                      return Enumerable.Empty<Type>();
                  }
              })
              .Where(t => t.IsClass && t.Namespace == typeof(Query).Namespace)
              .Where(t =>
              {
                  try
                  {
                      // Use TypeAttributes to avoid layout or offset-related exceptions
                      var fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                      return true; // No exception, type is safe
                  }
                  catch
                  {
                      return false; // Exception means type has problematic layout
                  }
              })
              .ToList();

        Console.WriteLine($"Total Types Found: {allMutations.Count}");

        foreach (var mutation in allMutations)
        {
            if (mutation.CustomAttributes.Any(a => a.AttributeType == typeof(GraphBaseAttribute)))
            {
                var entityName = mutation.CustomAttributes.First(a => a.AttributeType == typeof(GraphBaseAttribute))
                    .ConstructorArguments[0].Value.ToString();

                foreach (var methodType in Enum.GetValues(typeof(MethodTypes)).Cast<MethodTypes>()
                    .Where(a => !a.ToString().StartsWith("Get")))
                {
                    MethodInfo mi = mutation.GetMethods().FirstOrDefault(a => a.Name == methodType.ToString());
                    descriptor.Field(mi)
                        .Name(GetMethodName(entityName, methodType));
                }
            }
        }
    }

    private string GetMethodName(string entityName, MethodTypes methodType)
    {
        string simpleName = entityName.Singularize();
        if (methodType == MethodTypes.Update)
        {
            return $"{simpleName}_update{simpleName.FirstCharToUpper()}";
        }
        if (methodType == MethodTypes.Delete)
        {
            return $"{simpleName}_delete{simpleName.FirstCharToUpper()}";
        }
        return $"{simpleName}_create{simpleName.FirstCharToUpper()}";
    }
}
