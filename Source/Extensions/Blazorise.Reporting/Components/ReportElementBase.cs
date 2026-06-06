using System;
using System.Collections.Generic;
using Blazorise;
using Blazorise.Extensions;
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
    /// CSS class provider used to translate Blazorise utility parameters into CSS classes.
    /// </summary>
    [Inject] protected IClassProvider ClassProvider { get; set; }

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
            Class = BuildUtilityClasses(),
            Style = Style,
        };
    }

    /// <summary>
    /// Builds CSS classes from Blazorise utility parameters and custom classes.
    /// </summary>
    /// <returns>A space-separated CSS class list.</returns>
    protected string BuildUtilityClasses()
    {
        var classes = new List<string>();

        if ( !string.IsNullOrWhiteSpace( Class ) )
            classes.Add( Class );

        if ( Margin is not null )
            classes.Add( Margin.Class( ClassProvider ) );

        if ( Padding is not null )
            classes.Add( Padding.Class( ClassProvider ) );

        if ( Flex is not null )
            classes.Add( Flex.Class( ClassProvider ) );

        if ( Gap is not null )
            classes.Add( Gap.Class( ClassProvider ) );

        if ( TextColor.IsNotNullOrDefault() )
            classes.Add( ClassProvider.TextColor( TextColor ) );

        if ( Background.IsNotNullOrDefault() )
            classes.Add( ClassProvider.BackgroundColor( Background ) );

        return string.Join( " ", classes );
    }

    private ReportFontDefinition BuildFontDefinition()
    {
        return new()
        {
            Family = FontFamily ?? Font?.Family,
            Size = FontSize ?? Font?.Size,
            Color = FontColor ?? Font?.Color,
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
    /// Blazorise margin utility applied to the element.
    /// </summary>
    [Parameter] public IFluentSpacing Margin { get; set; }

    /// <summary>
    /// Blazorise padding utility applied to the element.
    /// </summary>
    [Parameter] public IFluentSpacing Padding { get; set; }

    /// <summary>
    /// Blazorise flex utility applied to the element.
    /// </summary>
    [Parameter] public IFluentFlex Flex { get; set; }

    /// <summary>
    /// Blazorise gap utility applied to the element.
    /// </summary>
    [Parameter] public IFluentGap Gap { get; set; }

    /// <summary>
    /// Blazorise text color utility applied to the element.
    /// </summary>
    [Parameter] public TextColor TextColor { get; set; } = TextColor.Default;

    /// <summary>
    /// Blazorise background color utility applied to the element.
    /// </summary>
    [Parameter] public Background Background { get; set; } = Background.Default;
}