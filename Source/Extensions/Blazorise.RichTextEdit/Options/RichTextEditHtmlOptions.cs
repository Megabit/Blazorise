namespace Blazorise.RichTextEdit;

/// <summary>
/// Represents options for retrieving HTML content from a rich text editor.
/// </summary>
public class RichTextEditHtmlOptions
{
    /// <summary>
    /// Gets a value indicating whether to use semantic HTML when retrieving the editor content.
    /// If <c>true</c>, the content will be retrieved using Quill's <c>getSemanticHTML()</c> method.
    /// If <c>false</c>, the content will be retrieved from the editor's inner HTML.
    /// </summary>
    public bool IsSemanticHtml { get; init; }

    /// <summary>
    /// Gets the starting index from which to retrieve the content.
    /// Only used if <see cref="IsSemanticHtml"/> is <c>true</c>.
    /// </summary>
    public int? Index { get; init; }

    /// <summary>
    /// Gets the optional length of content to retrieve.
    /// If not specified, the content will be retrieved from <see cref="Index"/> to the end of the document.
    /// Only used if <see cref="IsSemanticHtml"/> is <c>true</c>.
    /// </summary>
    public int? Length { get; init; }

    /// <summary>
    /// Creates a new <see cref="RichTextEditHtmlOptions"/> instance for retrieving raw inner HTML content.
    /// </summary>
    /// <returns>An options object with <see cref="IsSemanticHtml"/> set to <c>false</c>.</returns>
    public static RichTextEditHtmlOptions InnerHtml()
        => new() { IsSemanticHtml = false };

    /// <summary>
    /// Creates a new <see cref="RichTextEditHtmlOptions"/> instance for retrieving semantic HTML content.
    /// </summary>
    /// <param name="index">The starting index for the semantic HTML retrieval. Defaults to 0.</param>
    /// <param name="length">Optional length of content to retrieve. If <c>null</c>, retrieves until the end of the document.</param>
    /// <returns>An options object with <see cref="IsSemanticHtml"/> set to <c>true</c>, and the specified <paramref name="index"/> and <paramref name="length"/> values.</returns>
    public static RichTextEditHtmlOptions SemanticHtml( int index = 0, int? length = null ) =>
        new() { IsSemanticHtml = true, Index = index, Length = length };
}