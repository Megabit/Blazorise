#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid.Components;

/// <summary>
/// Internal PivotGrid pagination renderer.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public partial class _PivotGridPagination<TItem>
{
    #region Methods

    private Task SelectPage( string page )
        => PivotGrid.SelectPage( page );

    private Task SetPageSize( int pageSize )
        => PivotGrid.SetPageSize( pageSize );

    #endregion

    #region Properties

    private string PaginationStateKey
        => $"{CurrentPage}:{LastPage}:{PageSize}:{TotalRows}";

    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }

    /// <summary>
    /// Currently selected page.
    /// </summary>
    [Parameter] public int CurrentPage { get; set; }

    /// <summary>
    /// Last available page.
    /// </summary>
    [Parameter] public int LastPage { get; set; }

    /// <summary>
    /// Currently selected page size.
    /// </summary>
    [Parameter] public int PageSize { get; set; }

    /// <summary>
    /// Total number of pivot rows, or top-level groups when group paging is enabled.
    /// </summary>
    [Parameter] public int TotalRows { get; set; }

    #endregion
}