#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public partial class DataGridMultiSelectColumn<TItem> : DataGridColumn<TItem>
{
    #region Properties

    public override DataGridColumnType ColumnType => DataGridColumnType.MultiSelect;

    /// <summary>
    /// Template to customize multi select checkbox.
    /// </summary>
    [Parameter] public RenderFragment<MultiSelectContext<TItem>> MultiSelectTemplate { get; set; }

    [Parameter] public EventCallback<RowSelectionChangedEventArgs<TItem>> RowSelectionChanged { get; set; }

    #endregion
}