namespace Apsy.App.Propagator.Application.Extensions;

public static class EnumExtentions
{
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}