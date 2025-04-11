namespace Blazorise.Export;

/// <summary>
/// Represents the result of a text-based DataGrid export, including the exported text content.
/// </summary>
public interface ITextExportResult: IExportResult
{
    public string Text { get; init; }
}

/// <summary>
/// Concrete implementation of <see cref="ITextExportResult"/>, containing the exported text and success status.
/// </summary>
public class TextExportResult : ITextExportResult
{
    public bool IsSuccess { get; init; }
    public string Text { get; init; }
}
