using System;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Base class for declarative report elements that register themselves with the current report band.
/// </summary>
public abstract class ReportElementBase : ComponentBase
{
    private readonly string definitionId = Guid.NewGuid().ToString( "N" );

    [CascadingParameter] internal ReportSectionContext SectionContext { get; set; }

    /// <summary>
    /// Element kind represented by the derived component.
    /// </summary>
    protected abstract ReportElementType ElementType { get; }

    /// <summary>
    /// Element definition produced from the current component parameters.
    /// </summary>
    protected ReportElementDefinition Definition { get; private set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( SectionContext is null )
            return;

        Definition = BuildDefinition();
        SectionContext.Definition.Elements.Add( Definition );
    }

    /// <summary>
    /// Creates the element definition registered with the containing report band.
    /// </summary>
    /// <returns>A new element definition based on the component parameters.</returns>
    protected virtual ReportElementDefinition BuildDefinition()
    {
        return new()
        {
            Id = definitionId,
            Name = Name,
            Type = ElementType,
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            Font = BuildFontDefinition(),
            Appearance = BuildAppearanceDefinition(),
            Border = BuildBorderDefinition(),
            Class = Class,
            Style = Style,
        };
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

    private ReportAppearanceDefinition BuildAppearanceDefinition()
    {
        return new()
        {
            BackgroundColor = BackgroundColor ?? Appearance?.BackgroundColor,
            Background = GetBackgroundName( Background ) ?? Appearance?.Background,
            Opacity = Opacity ?? Appearance?.Opacity,
        };
    }

    private ReportBorderDefinition BuildBorderDefinition()
    {
        return new()
        {
            Color = BorderColor ?? Border?.Color,
            Width = BorderWidth ?? Border?.Width,
            Radius = BorderRadius ?? Border?.Radius,
        };
    }

    private static string GetTextColorName( TextColor textColor )
    {
        return textColor is null || textColor == Blazorise.TextColor.Default || string.IsNullOrWhiteSpace( textColor.RawName )
            ? null
            : textColor.RawName;
    }

    private static string GetBackgroundName( Background background )
    {
        return background is null || background == Blazorise.Background.Default || string.IsNullOrWhiteSpace( background.RawName )
            ? null
            : background.RawName;
    }

    /// <summary>
    /// Friendly element name shown in the designer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Horizontal position within the containing band.
    /// </summary>
    [Parameter] public double X { get; set; }

    /// <summary>
    /// Vertical position within the containing band.
    /// </summary>
    [Parameter] public double Y { get; set; }

    /// <summary>
    /// Element width in designer units.
    /// </summary>
    [Parameter] public double Width { get; set; } = 120;

    /// <summary>
    /// Element height in designer units.
    /// </summary>
    [Parameter] public double Height { get; set; } = 24;

    /// <summary>
    /// Additional CSS classes applied to the element.
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// Inline style applied to the element.
    /// </summary>
    [Parameter] public string Style { get; set; }

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
    /// Background color applied to the element.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; }

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
    /// Border color applied to the element.
    /// </summary>
    [Parameter] public string BorderColor { get; set; }

    /// <summary>
    /// Border width applied around the element.
    /// </summary>
    [Parameter] public double? BorderWidth { get; set; }

    /// <summary>
    /// Border radius applied to the element corners.
    /// </summary>
    [Parameter] public double? BorderRadius { get; set; }

    /// <summary>
    /// Element opacity from 0 to 1.
    /// </summary>
    [Parameter] public double? Opacity { get; set; }

    /// <summary>
    /// Font settings applied to text rendered by the element.
    /// </summary>
    [Parameter] public ReportFontDefinition Font { get; set; }

    /// <summary>
    /// Fill and opacity settings applied to the element.
    /// </summary>
    [Parameter] public ReportAppearanceDefinition Appearance { get; set; }

    /// <summary>
    /// Border settings applied around the element.
    /// </summary>
    [Parameter] public ReportBorderDefinition Border { get; set; }

    /// <summary>
    /// Blazorise margin utility accepted for declarative compatibility.
    /// </summary>
    [Parameter] public IFluentSpacing Margin { get; set; }

    /// <summary>
    /// Blazorise padding utility accepted for declarative compatibility.
    /// </summary>
    [Parameter] public IFluentSpacing Padding { get; set; }

    /// <summary>
    /// Blazorise flex utility accepted for declarative compatibility.
    /// </summary>
    [Parameter] public IFluentFlex Flex { get; set; }

    /// <summary>
    /// Blazorise gap utility accepted for declarative compatibility.
    /// </summary>
    [Parameter] public IFluentGap Gap { get; set; }

    /// <summary>
    /// Semantic Blazorise text color applied to text rendered by the element.
    /// </summary>
    [Parameter] public TextColor TextColor { get; set; } = TextColor.Default;

    /// <summary>
    /// Semantic Blazorise background color applied to the element fill.
    /// </summary>
    [Parameter] public Background Background { get; set; } = Background.Default;
}