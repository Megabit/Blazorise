namespace Blazorise.DataGrid;

/// <summary>
/// Defines the type of column filter operations.
/// </summary>
public enum DataGridColumnFilterMethod
{
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    NotEquals,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    Between,
}