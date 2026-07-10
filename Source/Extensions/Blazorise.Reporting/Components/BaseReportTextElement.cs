using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report elements that render text and support font settings.
/// </summary>
public abstract class BaseReportTextElement : BaseReportElement
{
    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportElementDefinition definition = base.BuildDefinition();
        definition.Font = BuildFontDefinition();

        return definition;
    }

    private ReportFontDefinition BuildFontDefinition()
    {
        return new()
        {
            Family = FontFamily,
            Size = FontSize,
            Color = FontColor,
            Bold = Bold,
            Italic = Italic,
            Underline = Underline,
            Alignment = TextAlignment,
            VerticalAlignment = VerticalAlignment,
        };
    }

    /// <summary>
    /// Font family applied to text rendered by the element.
    /// </summary>
    [Parameter] public string FontFamily { get; set; }

    /// <summary>
    /// Font size applied to text rendered by the element.
    /// </summary>
    [Parameter] public double? FontSize { get; set; }

    /// <summary>
    /// Text color applied to the element.
    /// </summary>
    [Parameter] public ReportColor FontColor { get; set; }

    /// <summary>
    /// Enables bold text rendering.
    /// </summary>
    [Parameter] public bool Bold { get; set; }

    /// <summary>
    /// Enables italic text rendering.
    /// </summary>
    [Parameter] public bool Italic { get; set; }

    /// <summary>
    /// Enables underline text rendering.
    /// </summary>
    [Parameter] public bool Underline { get; set; }

    /// <summary>
    /// Text alignment applied inside the element box.
    /// </summary>
    [Parameter] public TextAlignment TextAlignment { get; set; } = TextAlignment.Default;

    /// <summary>
    /// Vertical text alignment applied inside the element box.
    /// </summary>
    [Parameter] public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Default;

}