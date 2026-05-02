#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Describes the current pivot grid data request.
/// </summary>
public class PivotGridDataRequest
{
    /// <summary>
    /// Gets or sets row field states.
    /// </summary>
    public IReadOnlyList<PivotGridFieldState> Rows { get; set; } = [];

    /// <summary>
    /// Gets or sets column field states.
    /// </summary>
    public IReadOnlyList<PivotGridFieldState> Columns { get; set; } = [];

    /// <summary>
    /// Gets or sets aggregate field states.
    /// </summary>
    public IReadOnlyList<PivotGridFieldState> Aggregates { get; set; } = [];

    /// <summary>
    /// Gets or sets filter field states.
    /// </summary>
    public IReadOnlyList<PivotGridFieldState> Filters { get; set; } = [];

    /// <summary>
    /// Gets or sets the requested page.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the requested page size.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets whether paging is applied to top-level groups.
    /// </summary>
    public bool PageByGroups { get; set; }

    /// <summary>
    /// Gets or sets whether paging is enabled.
    /// </summary>
    public bool ShowPager { get; set; }

    /// <summary>
    /// Gets or sets whether row subtotals are requested.
    /// </summary>
    public bool ShowRowSubtotals { get; set; }

    /// <summary>
    /// Gets or sets whether column subtotals are requested.
    /// </summary>
    public bool ShowColumnSubtotals { get; set; }

    /// <summary>
    /// Gets or sets whether row totals are requested.
    /// </summary>
    public bool ShowRowTotals { get; set; }

    /// <summary>
    /// Gets or sets whether column totals are requested.
    /// </summary>
    public bool ShowColumnTotals { get; set; }

    /// <summary>
    /// Gets or sets the row total position.
    /// </summary>
    public PivotGridTotalPosition RowTotalPosition { get; set; }

    /// <summary>
    /// Gets or sets the column total position.
    /// </summary>
    public PivotGridTotalPosition ColumnTotalPosition { get; set; }

    /// <summary>
    /// Gets or sets whether row groups are expandable.
    /// </summary>
    public bool ExpandableRows { get; set; }

    /// <summary>
    /// Gets or sets whether column groups are expandable.
    /// </summary>
    public bool ExpandableColumns { get; set; }

    /// <summary>
    /// Gets or sets whether expandable groups are initially expanded.
    /// </summary>
    public bool InitiallyExpanded { get; set; }

    /// <summary>
    /// Gets or sets whether the provider should return raw data items for local pivoting.
    /// </summary>
    public bool RequiresRawData { get; set; } = true;
}