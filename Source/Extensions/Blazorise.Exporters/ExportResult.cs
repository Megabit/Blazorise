namespace Blazorise.Exporters;

/// <summary>
/// Default implementation of <see cref="IExportResult"/>, representing the outcome of an export operation.
/// </summary>
public class ExportResult : IExportResult
{
    /// <summary>
    /// Gets a value indicating whether the export operation was successful.
    /// </summary>
    public bool Success => Errors is null || Errors.Length == 0;

    /// <summary>
    /// Errors from export operation
    /// </summary>
    public string[] Errors { get; init; } = { };
}