using Aps.CommonBack.Base.Attributes;
using Aps.CommonBack.Base.Enums;
using Aps.CommonBack.Base.Extensions;
using Humanizer;
using System.Reflection;

namespace Apsy.App.Propagator.Api.GraphQL.ObjectTypes;

public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
            var allQueries = AppDomain.CurrentDomain.GetAssemblies()
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

            Console.WriteLine($"Total Types Found: {allQueries.Count}");
            foreach (var query in allQueries)
        {
            if (query.CustomAttributes.Any(a => a.AttributeType == typeof(GraphBaseAttribute)))
            {
                var entityName = query.CustomAttributes.First(a => a.AttributeType == typeof(GraphBaseAttribute))
                    .ConstructorArguments[0].Value.ToString();

                foreach (var methodType in Enum.GetValues(typeof(MethodTypes)).Cast<MethodTypes>()
                    .Where(a => a.ToString().StartsWith("Get")))
                {
                    MethodInfo mi = query.GetMethods().FirstOrDefault(a => a.Name == methodType.ToString());
                    descriptor.Field(mi)
                        .Name(GetMethodName(entityName, methodType));
                }
            }
        }
    }

    private string GetMethodName(string entityName, MethodTypes methodType)
    {
        string simpleName = entityName.Pluralize();
        if (methodType == MethodTypes.Get)
        {
            return $"{simpleName.FirstCharToLower().Singularize()}_get{simpleName.Singularize()}";
        }
        return $"{simpleName.FirstCharToLower().Singularize()}_get{simpleName}";
    }
}
