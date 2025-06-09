namespace Apsy.App.Propagator.Application.Extensions;

public class ReflectionExtentions
{
    public static List<Type> LoadTypesFromAssemblies(Func<Type, bool> predicate)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(predicate).ToList();

        return types;
    }
}
