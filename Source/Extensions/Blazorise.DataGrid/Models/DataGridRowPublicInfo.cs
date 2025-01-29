namespace Blazorise.DataGrid;

public class DataGridRowPublicInfo<TItem>
{
    private readonly DataGridRowInfo<TItem> _dataGridRowInfo;
    public DataGridRowPublicInfo(DataGridRowInfo<TItem> dataGridRowInfo  )
    {
        _dataGridRowInfo = dataGridRowInfo;
    }
    public bool DetailRowExpanded => _dataGridRowInfo.DetailRowExpanded;
}