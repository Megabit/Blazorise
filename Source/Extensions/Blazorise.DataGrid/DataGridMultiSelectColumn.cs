#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public partial class DataGridMultiSelectColumn<TItem> : DataGridColumn<TItem>
{
    #region Constructors

    public DataGridMultiSelectColumn()
    {
        // Avoid row click side-effects when interacting with the header/body checkboxes.
        PreventRowClick = true;
    }

    #endregion

    #region Properties

    public override DataGridColumnType ColumnType => DataGridColumnType.MultiSelect;

    /// <summary>
    /// Template to customize multi select checkbox.
    /// </summary>
    [Parameter] public RenderFragment<MultiSelectContext<TItem>> MultiSelectTemplate { get; set; }

    #endregion
}