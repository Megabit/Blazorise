namespace Blazorise.Extensions;

/// <summary>
/// Various extension methods for numeric object.
/// </summary>
public static class NumericExtensions
{
    /// <summary>
    /// Converts the value to a precise string representation of the number without rounding it.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representation of the value with the specified number of decimal places.</returns>
    public static string ToPreciseString( this double value )
    {
        return value.ToString( "G29" );
    }

    /// <summary>
    /// Converts the value to a precise string representation of the number without rounding it.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A string representation of the value with the specified number of decimal places.</returns>
    public static string ToPreciseString( this double? value )
    {
        return value?.ToString( "G29" );
    }
}
