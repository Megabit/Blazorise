namespace Blazorise.DataGrid;

public class DataGridRowInfo<TItem>
{
    private readonly _DataGridRowInfo<TItem> _dataGridRowInfo;
    public DataGridRowInfo(_DataGridRowInfo<TItem> dataGridRowInfo  )
    {
        _dataGridRowInfo = dataGridRowInfo;
    }
    public bool DetailRowExpanded => _dataGridRowInfo.DetailRowExpanded;
}