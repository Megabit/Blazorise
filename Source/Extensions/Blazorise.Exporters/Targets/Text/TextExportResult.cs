namespace Blazorise.Exporters;

/// <summary>
/// Concrete implementation of <see cref="ITextExportResult"/>, containing the exported text and success status.
/// </summary>
public class TextExportResult : ITextExportResult
{
    /// <summary>
    /// Gets the success status of the export operation.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the exported text content.
    /// </summary>
    public string Text { get; init; }
}