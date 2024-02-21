namespace Blazorise.DataGrid.EventArguments;
public class ColumnDisplayChangedEventArgs<TItem>
{
    public DataGridColumn<TItem> Column { get; set; }
    public bool Display { get; set; }

    public ColumnDisplayChangedEventArgs( DataGridColumn<TItem> column, bool display )
    {
        Column = column;
        Display = display;
    }
}
