#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid detail row rendering and interaction in a DataGrid.
/// </summary>
public abstract class _BaseDataGridDetailRow<TItem> : BaseDataGridComponent
{
    #region Properties

    /// <summary>
    /// Controls member behavior for the base data grid detail row.
    /// </summary>
    protected bool HasCommandColumn
        => Columns.Any( x => x.ColumnType == DataGridColumnType.Command );

    /// <summary>
    /// Number of grid columns covered by the detail row.
    /// </summary>
    protected int ColumnSpan
        => Columns.Where( x => x.Displayable ).Count() - ( HasCommandColumn && !ParentDataGrid.Editable ? 1 : 0 );

    /// <summary>
    /// Item consumed by the data set.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// List of columns used to build this row.
    /// </summary>
    [Parameter] public IReadOnlyList<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Renders the expanded details for the current item.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}