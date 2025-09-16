namespace Blazorise.DataGrid;

/// <summary>
/// Defines the type of filter operations.
/// </summary>
public enum DataGridFilterMethod
{
    /// <summary>
    /// Defaults to the column specific default filter method.
    /// </summary>
    Default,

    /// <summary>
    /// Determines whether the specified substring occurs within this string.
    /// </summary>
    Contains,

    /// <summary>
    /// Determines whether the beginning of this string matches the specified substring.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Determines whether the end of this string matches the specified substring.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Determines whether two values are equal.
    /// </summary>
    Equals,

    /// <summary>
    /// Determines whether the specified values are not equal.
    /// </summary>
    NotEquals,
}
