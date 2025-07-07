namespace Blazorise.Exporters;

/// <summary>
/// Concrete implementation of <see cref="ITextExportResult"/>, containing the exported text and success status.
/// </summary>
public class TextExportResult : ITextExportResult
{
    ///<inheritdoc/>
    public bool Success { get; init; }

    ///<inheritdoc/>
    public string[] Errors { get; init; }

    /// <summary>
    /// Gets the exported text content.
    /// </summary>
    public string Text { get; init; }
}