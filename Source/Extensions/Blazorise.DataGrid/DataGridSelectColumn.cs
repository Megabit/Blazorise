namespace Blazorise.DataGrid
{
    public class DataGridSelectColumn<TItem> : DataGridColumn<TItem>
    {
        public override DataGridColumnType ColumnType => DataGridColumnType.Select;
    }
}