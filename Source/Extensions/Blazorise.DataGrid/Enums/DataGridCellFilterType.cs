namespace Blazorise.DataGrid;

/// <summary>
/// Defines the DataGrid cell filter type.
/// </summary>
public enum DataGridCellFilterType
{
    /// <summary>
    /// Represents a single filter, allowing only one value.
    /// </summary>
    Single,

    /// <summary>
    /// Represents the starting value of a range filter.
    /// </summary>
    RangeFrom,

    /// <summary>
    /// Represents the ending value of a range filter.
    /// </summary>
    RangeTo,
}