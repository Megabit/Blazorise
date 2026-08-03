namespace Blazorise.DataGrid;

/// <summary>
/// Defines the type of column filter operations.
/// </summary>
public enum DataGridColumnFilterMethod
{
    /// <summary>
    /// Matches column values with the contains comparison.
    /// </summary>
    Contains,
    /// <summary>
    /// Matches column values with the starts with comparison.
    /// </summary>
    StartsWith,
    /// <summary>
    /// Matches column values with the ends with comparison.
    /// </summary>
    EndsWith,
    /// <summary>
    /// Matches column values with the equals comparison.
    /// </summary>
    Equals,
    /// <summary>
    /// Matches column values with the not equals comparison.
    /// </summary>
    NotEquals,
    /// <summary>
    /// Matches column values with the less than comparison.
    /// </summary>
    LessThan,
    /// <summary>
    /// Matches column values with the less than or equal comparison.
    /// </summary>
    LessThanOrEqual,
    /// <summary>
    /// Matches column values with the greater than comparison.
    /// </summary>
    GreaterThan,
    /// <summary>
    /// Matches column values with the greater than or equal comparison.
    /// </summary>
    GreaterThanOrEqual,
    /// <summary>
    /// Matches column values with the between comparison.
    /// </summary>
    Between,
}