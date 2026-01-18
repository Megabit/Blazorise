#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

public abstract class _BaseDataGridDetailRow<TItem> : BaseDataGridComponent
{
    #region Properties

    protected bool HasCommandColumn
        => Columns.Any( x => x.ColumnType == DataGridColumnType.Command );

    protected int ColumnSpan
        => Columns.Where( x => x.Displayable ).Count() - ( HasCommandColumn && !ParentDataGrid.Editable ? 1 : 0 );

    /// <summary>
    /// Item associated with the data set.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// List of columns used to build this row.
    /// </summary>
    [Parameter] public IReadOnlyList<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}