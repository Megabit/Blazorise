namespace Blazorise.Exporters;

/// <summary>
/// Represents a generic result of an export operation, indicating whether the export was successful.
/// </summary>
public interface IExportResult
{
    /// <summary>
    /// Indicates whether an operation was successful.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Collection of error messages generated during an export operation.
    /// </summary>
    string[] Errors { get; init; }
}