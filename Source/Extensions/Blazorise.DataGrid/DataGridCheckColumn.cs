namespace Blazorise.DataGrid
{
    public class DataGridCheckColumn<TItem> : DataGridColumn<TItem>
    {
        public override DataGridColumnType ColumnType => DataGridColumnType.Check;
    }
}