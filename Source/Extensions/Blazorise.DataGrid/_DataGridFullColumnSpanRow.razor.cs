#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public abstract class _BaseDataGridFullColumnSpanRow<TItem> : BaseDataGridComponent
{
    #region Properties

    protected string GetStyle()
        => $"background-color: unset; {Style}";

    protected override bool ShouldRender()
        => RenderUpdates;

    protected bool HasCommandColumn
        => Columns.Any( x => x.ColumnType == DataGridColumnType.Command );

    protected int ColumnSpan
        => Columns.Count - ( HasCommandColumn && !ParentDataGrid.Editable ? 1 : 0 );

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

    [Parameter] public bool RenderUpdates { get; set; }

    [Parameter] public string Style { get; set; }

    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Defines the element position.
    /// </summary>
    [Parameter] public IFluentPosition Position { get; set; }

    [Parameter] public Background Background { get; set; }

    [Parameter]
    public IFluentSpacing Padding { get; set; }

    [Parameter]
    public IFluentSpacing Margin { get; set; }

    #endregion
}