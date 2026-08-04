using Microsoft.AspNetCore.Components;

namespace Blazorise.Pdf;

/// <summary>
/// Defines text content in a PDF document.
/// </summary>
public class PdfText : BasePdfElement
{
    #region Methods

    /// <inheritdoc />
    protected override void UpdateDefinition( PdfElementDefinition definition )
    {
        base.UpdateDefinition( definition );

        definition.Text = Text;
        definition.Wrap = Wrap;

        definition.Font ??= new();
        definition.Font.Family = FontFamily;
        definition.Font.Size = FontSize;
        definition.Font.Color = TextColor;
        definition.Font.Alignment = TextAlignment;
        definition.Font.VerticalAlignment = VerticalAlignment;
        definition.Font.Bold = Bold;
        definition.Font.Italic = Italic;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Text;

    /// <inheritdoc />
    protected override bool ElementClipContent => ClipContent;

    /// <inheritdoc />
    protected override string ElementBackgroundColor => BackgroundColor;

    /// <summary>
    /// Text rendered by the element.
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// Indicates that text should wrap inside the element bounds.
    /// </summary>
    [Parameter] public bool Wrap { get; set; } = true;

    /// <summary>
    /// Indicates that content should be clipped to the element bounds.
    /// </summary>
    [Parameter] public bool ClipContent { get; set; } = true;

    /// <summary>
    /// Font family used by the text. The built-in renderer maps the family to the closest PDF standard font (Helvetica, Times, or Courier).
    /// </summary>
    [Parameter] public string FontFamily { get; set; } = "Helvetica";

    /// <summary>
    /// Font size used by the text.
    /// </summary>
    [Parameter] public double FontSize { get; set; } = 12;

    /// <summary>
    /// Text color in hexadecimal format.
    /// </summary>
    [Parameter] public string TextColor { get; set; } = "#000000";

    /// <summary>
    /// Text alignment inside the element bounds.
    /// </summary>
    /// <remarks>
    /// <see cref="TextAlignment.Default"/> and <see cref="TextAlignment.Start"/> align to the start.
    /// <see cref="TextAlignment.Justified"/> distributes words across wrapped non-final paragraph lines.
    /// </remarks>
    [Parameter] public TextAlignment TextAlignment { get; set; }

    /// <summary>
    /// Text vertical alignment inside the element bounds.
    /// </summary>
    /// <remarks>
    /// <see cref="VerticalAlignment.Default"/>, <see cref="VerticalAlignment.Baseline"/>,
    /// <see cref="VerticalAlignment.Top"/>, and <see cref="VerticalAlignment.TextTop"/> align to the top.
    /// <see cref="VerticalAlignment.Middle"/> centers the text, while <see cref="VerticalAlignment.Bottom"/>
    /// and <see cref="VerticalAlignment.TextBottom"/> align to the bottom.
    /// </remarks>
    [Parameter] public VerticalAlignment VerticalAlignment { get; set; }

    /// <summary>
    /// Makes text bold.
    /// </summary>
    [Parameter] public bool Bold { get; set; }

    /// <summary>
    /// Makes text italic.
    /// </summary>
    [Parameter] public bool Italic { get; set; }

    /// <summary>
    /// Background color in hexadecimal format.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

    #endregion
}