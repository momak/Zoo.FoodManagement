namespace Zoo.Common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Extension method to return an enum value of type T for the given string.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">input value to evaluate</param>
    /// <returns>enum value</returns>
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// Extension method to return an enum value of type T for the given int.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">input value to evaluate</param>
    /// <returns>enum value</returns>
    public static T ToEnum<T>(this int value)
    {
        var name = Enum.GetName(typeof(T), value);
        return name.ToEnum<T>();
    }
}