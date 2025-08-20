namespace Blazorise.Exporters;

/// <summary>
/// Represents the result of a text-based export, including the exported text content.
/// </summary>
public interface ITextExportResult : IExportResult
{
    /// <summary>
    /// Gets the exported text content.
    /// </summary>
    public string Text { get; init; }
}