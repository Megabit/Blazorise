namespace Blazorise.Markdown;

/// <summary>
/// Component classes for <see cref="Markdown"/>.
/// </summary>
public sealed record MarkdownClasses : ComponentClasses
{
    /// <summary>
    /// Targets the text area element.
    /// </summary>
    public string TextArea { get; set; }
}

/// <summary>
/// Component styles for <see cref="Markdown"/>.
/// </summary>
public sealed record MarkdownStyles : ComponentStyles
{
    /// <summary>
    /// Targets the text area element.
    /// </summary>
    public string TextArea { get; set; }
}