using System;
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
        Blazorise.VerticalAlignment verticalAlignment = this.VerticalAlignment != Blazorise.VerticalAlignment.Default
            ? this.VerticalAlignment
            : Font?.VerticalAlignment ?? Blazorise.VerticalAlignment.Default;

        return new()
        {
            Family = FontFamily ?? Font?.Family,
            Size = FontSize ?? Font?.Size,
            Color = FontColor.IsDefault ? Font?.Color ?? ReportColor.Default : FontColor,
            Bold = Bold || Font?.Bold == true,
            Italic = Italic || Font?.Italic == true,
            Underline = Underline || Font?.Underline == true,
            Alignment = TextAlignment != Blazorise.TextAlignment.Default
                ? TextAlignment
                : Font?.Alignment ?? Blazorise.TextAlignment.Default,
            VerticalAlignment = verticalAlignment,
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
    [Parameter] public Blazorise.VerticalAlignment VerticalAlignment { get; set; } = Blazorise.VerticalAlignment.Default;

    /// <summary>
    /// Font settings applied to text rendered by the element.
    /// </summary>
    [Parameter] public ReportFontDefinition Font { get; set; }

}