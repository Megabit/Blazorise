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
    /// Gets or sets how data is being requested. When set to <see cref="PivotGridReadDataMode.Virtualize"/>, providers should use <see cref="VirtualizeOffset"/> and <see cref="VirtualizeCount"/> to return the requested pivot result row range.
    /// </summary>
    public PivotGridReadDataMode ReadDataMode { get; set; }

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
    /// Gets or sets the requested virtualized data start index when <see cref="ReadDataMode"/> is <see cref="PivotGridReadDataMode.Virtualize"/>.
    /// </summary>
    public int VirtualizeOffset { get; set; }

    /// <summary>
    /// Gets or sets the requested virtualized data item count when <see cref="ReadDataMode"/> is <see cref="PivotGridReadDataMode.Virtualize"/>.
    /// </summary>
    public int VirtualizeCount { get; set; }

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
    /// Gets or sets whether row totals are requested as total columns.
    /// </summary>
    public bool ShowRowTotals { get; set; }

    /// <summary>
    /// Gets or sets whether column totals are requested as total rows.
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

    internal IReadOnlyList<IReadOnlyList<object>> CollapsedRowGroupPaths { get; set; } = [];

    internal IReadOnlyList<IReadOnlyList<object>> ExpandedRowGroupPaths { get; set; } = [];

    internal IReadOnlyList<IReadOnlyList<object>> CollapsedColumnGroupPaths { get; set; } = [];

    internal IReadOnlyList<IReadOnlyList<object>> ExpandedColumnGroupPaths { get; set; } = [];

    /// <summary>
    /// Gets whether the row group path is expanded for the current request.
    /// </summary>
    /// <param name="path">The row group value path.</param>
    /// <returns><c>true</c> when the row group is expanded.</returns>
    public bool IsRowGroupExpanded( IReadOnlyList<object> path )
        => IsGroupExpanded( path, InitiallyExpanded, CollapsedRowGroupPaths, ExpandedRowGroupPaths );

    /// <summary>
    /// Gets whether the column group path is expanded for the current request.
    /// </summary>
    /// <param name="path">The column group value path.</param>
    /// <returns><c>true</c> when the column group is expanded.</returns>
    public bool IsColumnGroupExpanded( IReadOnlyList<object> path )
        => IsGroupExpanded( path, InitiallyExpanded, CollapsedColumnGroupPaths, ExpandedColumnGroupPaths );

    private static bool IsGroupExpanded( IReadOnlyList<object> path, bool initiallyExpanded, IReadOnlyList<IReadOnlyList<object>> collapsedGroupPaths, IReadOnlyList<IReadOnlyList<object>> expandedGroupPaths )
    {
        if ( path is null )
            return false;

        return initiallyExpanded
            ? !ContainsPath( collapsedGroupPaths, path )
            : ContainsPath( expandedGroupPaths, path );
    }

    private static bool ContainsPath( IReadOnlyList<IReadOnlyList<object>> paths, IReadOnlyList<object> path )
    {
        if ( paths is null )
            return false;

        foreach ( IReadOnlyList<object> candidate in paths )
        {
            if ( ValuesEqual( candidate, path ) )
                return true;
        }

        return false;
    }

    private static bool ValuesEqual( IReadOnlyList<object> left, IReadOnlyList<object> right )
    {
        if ( left is null || right is null )
            return left is null && right is null;

        if ( left.Count != right.Count )
            return false;

        for ( var i = 0; i < left.Count; i++ )
        {
            if ( !object.Equals( left[i], right[i] ) )
                return false;
        }

        return true;
    }
}