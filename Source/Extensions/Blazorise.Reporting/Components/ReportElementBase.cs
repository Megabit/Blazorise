using System;
using System.Collections.Generic;
using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

public abstract class ReportElementBase : ComponentBase
{
    private readonly string definitionId = Guid.NewGuid().ToString( "N" );

    [CascadingParameter] internal ReportSectionContext SectionContext { get; set; }

    [Inject] protected IClassProvider ClassProvider { get; set; }

    protected abstract ReportElementType ElementType { get; }

    protected ReportElementDefinition Definition { get; private set; }

    protected override void OnParametersSet()
    {
        if ( SectionContext is null )
            return;

        Definition = BuildDefinition();
        SectionContext.Definition.Elements.Add( Definition );
    }

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

    [Parameter] public string Name { get; set; }

    [Parameter] public double X { get; set; }

    [Parameter] public double Y { get; set; }

    [Parameter] public double Width { get; set; } = 120;

    [Parameter] public double Height { get; set; } = 24;

    [Parameter] public string Class { get; set; }

    [Parameter] public string Style { get; set; }

    [Parameter] public IFluentSpacing Margin { get; set; }

    [Parameter] public IFluentSpacing Padding { get; set; }

    [Parameter] public IFluentFlex Flex { get; set; }

    [Parameter] public IFluentGap Gap { get; set; }

    [Parameter] public TextColor TextColor { get; set; } = TextColor.Default;

    [Parameter] public Background Background { get; set; } = Background.Default;
}