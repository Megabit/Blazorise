namespace Blazorise.Exporters;

public class TabularSourceData<TDataType>: IExportableData<TDataType>
{
    public List<List<TDataType>> Data { get; set; } = new();
    public List<string> ColumnNames { get; set; } = new();
}
