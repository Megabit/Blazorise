namespace Blazorise.Export;

/// <summary>
/// Represents a generic result of a DataGrid export operation, indicating whether the export was successful.
/// </summary>
public interface IExportResult
{
    bool Success { get; init; }
}

/// <summary>
/// Default implementation of <see cref="IExportResult"/>, representing the outcome of a DataGrid export operation.
/// </summary>
public class ExportResult : IExportResult
{
    public bool Success { get; init; }
}



