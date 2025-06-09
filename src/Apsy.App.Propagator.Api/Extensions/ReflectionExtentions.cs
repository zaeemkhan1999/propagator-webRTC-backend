namespace Apsy.App.Propagator.Api.Extensions;

public class ReflectionExtentions
{
    public static List<Type> LoadTypesFromAssemblies(Func<Type, bool> predicate)
    {
        //var types = Assembly.GetExecutingAssembly().GetTypes().Where(predicate).ToList();
        var types=AppDomain.CurrentDomain.GetAssemblies().SelectMany(t=>t.GetTypes().Where(predicate)).ToList();

        return types;
    }
}
