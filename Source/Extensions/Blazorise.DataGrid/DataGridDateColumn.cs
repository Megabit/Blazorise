using Microsoft.AspNetCore.Components;

namespace Blazorise.DataGrid
{
    public class DataGridDateColumn<TItem> : DataGridColumn<TItem>
    {
        public override DataGridColumnType ColumnType => DataGridColumnType.Date;

        /// <summary>
        /// Hints at the type of data that might be entered by the user while editing the element or its contents.
        /// </summary>
        [Parameter] public DateInputMode InputMode { get; set; }
    }
}