using System;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report elements that render text and support font settings.
/// </summary>
public abstract class ReportTextElementBase : ReportElementBase
{
    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        var definition = base.BuildDefinition();
        definition.Font = BuildFontDefinition();
        return definition;
    }

    private ReportFontDefinition BuildFontDefinition()
    {
        return new()
        {
            Family = FontFamily ?? Font?.Family,
            Size = FontSize ?? Font?.Size,
            Color = FontColor ?? Font?.Color,
            TextColor = GetTextColorName( TextColor ) ?? Font?.TextColor,
            Bold = Bold || Font?.Bold == true,
            Italic = Italic || Font?.Italic == true,
            Underline = Underline || Font?.Underline == true,
            Alignment = TextAlignment != Blazorise.TextAlignment.Default
                ? TextAlignment
                : Font?.Alignment ?? Blazorise.TextAlignment.Default,
        };
    }

    private static string GetTextColorName( TextColor textColor )
    {
        return textColor is null || textColor == Blazorise.TextColor.Default || string.IsNullOrWhiteSpace( textColor.RawName )
            ? null
            : textColor.RawName;
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
    [Parameter] public string FontColor { get; set; }

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
    /// Font settings applied to text rendered by the element.
    /// </summary>
    [Parameter] public ReportFontDefinition Font { get; set; }

    /// <summary>
    /// Semantic Blazorise text color applied to text rendered by the element.
    /// </summary>
    [Parameter] public TextColor TextColor { get; set; } = TextColor.Default;
}