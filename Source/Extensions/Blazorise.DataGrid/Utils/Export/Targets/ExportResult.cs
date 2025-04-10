namespace Blazorise.DataGrid;

/// <summary>
/// Represents a generic result of a DataGrid export operation, indicating whether the export was successful.
/// </summary>
public interface IExportResult
{
    bool IsSuccess { get; init; }
}

/// <summary>
/// Default implementation of <see cref="IExportResult"/>, representing the outcome of a DataGrid export operation.
/// </summary>
public class ExportResult : IExportResult
{
    public bool IsSuccess { get; init; }
}



