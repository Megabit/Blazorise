#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents data returned by an external pivot grid data provider.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridDataResult<TItem>
{
    /// <summary>
    /// Gets or sets raw data items used by the pivot grid to build the result locally. In virtualized result requests, prefer returning <see cref="Result"/> because partial raw data can produce incomplete aggregates.
    /// </summary>
    public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets or sets total item count reported by the provider. For virtualized result requests, this is the total number of pivot result rows.
    /// </summary>
    public int? TotalItems { get; set; }

    /// <summary>
    /// Gets or sets whether returned data has already been paged by the provider.
    /// </summary>
    public bool IsPaged { get; set; }

    /// <summary>
    /// Gets or sets an already prepared pivot result. In virtualized result requests, this should contain the requested pivot result rows.
    /// </summary>
    public PivotGridResult<TItem> Result { get; set; }
}