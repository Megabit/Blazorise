#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid full column span row rendering and interaction in a DataGrid.
/// </summary>
public abstract class _BaseDataGridFullColumnSpanRow<TItem> : BaseDataGridComponent
{
    #region Properties

    /// <inheritdoc />
    protected override bool ShouldRender()
        => RenderUpdates;

    /// <summary>
    /// Number of grid columns covered by this row.
    /// </summary>
    protected int ColumnSpan
        => Columns.Count( IsColumnVisible );

    private bool IsColumnVisible( DataGridColumn<TItem> column )
    {
        if ( !( column.IsDisplayable || column.Displaying ) )
            return false;

        if ( column.IsCommandColumn )
            return ParentDataGrid.IsCommandVisible || ParentDataGrid.EditMode is DataGridEditMode.Inline or DataGridEditMode.Cell;

        if ( column.IsMultiSelectColumn )
            return ParentDataGrid.MultiSelect;

        return true;
    }

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
    /// Renders content spanning the visible grid columns.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Allows the spanning row to refresh after state changes.
    /// </summary>
    [Parameter] public bool RenderUpdates { get; set; }

    #endregion
}