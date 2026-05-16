namespace Blazorise.PivotGrid;

/// <summary>
/// Specifies how PivotGrid data is requested from an external data provider.
/// </summary>
public enum PivotGridReadDataMode
{
    /// <summary>
    /// Requests all data needed to build the pivot result.
    /// </summary>
    All,

    /// <summary>
    /// Requests data for a page.
    /// </summary>
    Paging,

    /// <summary>
    /// Requests data for a virtualized range.
    /// </summary>
    Virtualize
}