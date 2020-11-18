#region Using directives

using Microsoft.AspNetCore.Components;

#endregion Using directives

namespace Blazorise.DataGrid
{
    public partial class DataGridMultiSelectColumn<TItem> : DataGridColumn<TItem>
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        public override DataGridColumnType ColumnType => DataGridColumnType.MultiSelect;

        /// <summary>
        /// Template to customize multi select checkbox.
        /// </summary>
        [Parameter] public RenderFragment<MultiSelectContext<TItem>> MultiSelectTemplate { get; set; }

        #endregion
    }
}