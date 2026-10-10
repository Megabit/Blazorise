namespace Blazorise.Demo.Models;

/// <summary>
/// Sample values displayed by the ribbon demo's controls and document preview.
/// </summary>
public record WordRibbonDocument
{
    public string Title { get; init; } = "Quarterly report";

    public string Text { get; init; } = "Quarterly overview\n\nOur team made steady progress across product development, customer experience, and operations. This report brings together the highlights and priorities for the next quarter.\n\nHighlights\n\n• Delivered the new customer portal.\n• Improved response times across support channels.\n• Expanded the product research program.\n\nNext steps\n\nContinue investing in a clear, consistent experience for our customers and our team.";

    public string FontFamily { get; init; } = "Segoe UI";

    public int FontSize { get; init; } = 14;

    public bool Bold { get; init; }

    public bool Italic { get; init; }

    public bool Underline { get; init; }

    public bool Strikethrough { get; init; }

    public string FontColor { get; init; } = "#222222";

    public string HighlightColor { get; init; } = "#ffffff";

    public TextAlignment Alignment { get; init; } = TextAlignment.Start;

    public string Style { get; init; } = "Normal";
}