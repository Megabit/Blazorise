namespace Blazorise.DataGrid;

public class DataGridRowPublicInfo<TItem>
{
    private readonly _DataGridRowInfo<TItem> _dataGridRowInfo;
    public DataGridRowPublicInfo(_DataGridRowInfo<TItem> dataGridRowInfo  )
    {
        _dataGridRowInfo = dataGridRowInfo;
    }
    public bool DetailRowExpanded => _dataGridRowInfo.DetailRowExpanded;
}