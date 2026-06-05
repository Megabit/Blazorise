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