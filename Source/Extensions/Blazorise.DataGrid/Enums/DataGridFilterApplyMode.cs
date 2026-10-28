namespace Blazorise.DataGrid;

/// <summary>
/// Defines when column filter changes are applied to the data grid.
/// </summary>
public enum DataGridFilterApplyMode
{
    /// <summary>
    /// Uses the existing filter row and filter menu behavior.
    /// </summary>
    Immediate,

    /// <summary>
    /// Defers column filter changes until ApplyFilters is called.
    /// </summary>
    Deferred,
}