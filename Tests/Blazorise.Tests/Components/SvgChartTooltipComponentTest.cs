#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Charts.Svg;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartTooltipComponentTest : BunitContext
{
    public SvgChartTooltipComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseSvgChart();
    }

    [Fact]
    public void IntersectFalse_RendersCategoryHitRegionsAndShowsIndexItems()
    {
        IRenderedComponent<SvgLineChart<TooltipSample>> component = RenderChart( false );

        var hitRegions = component.FindAll( ".svg-chart-tooltip-hit-region" );

        Assert.Equal( 2, hitRegions.Count );

        hitRegions[0].MouseEnter();

        component.WaitForAssertion( () =>
        {
            Assert.Contains( "First: 10", component.Markup );
            Assert.Contains( "Second: 20", component.Markup );
            Assert.Equal( "div", component.Find( ".svg-chart-tooltip" ).LocalName );
            Assert.Empty( component.FindAll( "svg .svg-chart-tooltip" ) );
        } );
    }

    [Fact]
    public void IntersectTrue_DoesNotRenderCategoryHitRegions()
    {
        IRenderedComponent<SvgLineChart<TooltipSample>> component = RenderChart( true );

        Assert.Empty( component.FindAll( ".svg-chart-tooltip-hit-region" ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void Tooltip_InheritsSizingFontAndPaddingForTemplates( bool autoSize )
    {
        var component = RenderChart( false, new()
        {
            AutoSize = autoSize,
            Width = 240,
            Height = 80,
            Font = new() { Size = 28 },
            Padding = new() { Top = 2, End = 4, Bottom = 6, Start = 8 },
            Template = context => builder => builder.AddContent( 0, $"Custom {context.Items.Count} items" ),
        } );

        component.Find( ".svg-chart-tooltip-hit-region" ).MouseEnter();

        component.WaitForAssertion( () =>
        {
            var tooltip = component.Find( ".svg-chart-tooltip" );
            var content = component.Find( ".svg-chart-tooltip-content" );
            Assert.Contains( "Custom 2 items", content.TextContent );
            Assert.Contains( "font-size:28px", content.GetAttribute( "style" ) );
            Assert.Contains( "padding:2px 4px 6px 8px", content.GetAttribute( "style" ) );

            if ( autoSize )
            {
                Assert.Contains( "width:max-content", tooltip.GetAttribute( "style" ) );
                Assert.Contains( "min-height:80px", tooltip.GetAttribute( "style" ) );
                Assert.DoesNotContain( "overflow:hidden", content.GetAttribute( "style" ) );
            }
            else
            {
                Assert.Contains( "width:240px", tooltip.GetAttribute( "style" ) );
                Assert.Contains( "height:80px", tooltip.GetAttribute( "style" ) );
                Assert.Contains( "overflow:hidden", content.GetAttribute( "style" ) );
            }
        } );
    }

    private IRenderedComponent<SvgLineChart<TooltipSample>> RenderChart( bool intersect, SvgChartTooltipOptions tooltip = null )
    {
        List<TooltipSample> samples =
        [
            new() { Category = "A", First = 10, Second = 20 },
            new() { Category = "B", First = 15, Second = 25 }
        ];

        return Render<SvgLineChart<TooltipSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .Add( chart => chart.Options, new SvgChartOptions { Tooltip = tooltip ?? new() } )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgChartTooltip>( 0 );
                builder.AddAttribute( 1, nameof( SvgChartTooltip.InteractionMode ), SvgChartInteractionMode.Index );
                builder.AddAttribute( 2, nameof( SvgChartTooltip.Intersect ), intersect );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<TooltipSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartCategoryAxis<TooltipSample>.Value ), (Func<TooltipSample, object>)( item => item.Category ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgLineSeries<TooltipSample>>( 5 );
                builder.AddAttribute( 6, nameof( SvgLineSeries<TooltipSample>.Name ), "First" );
                builder.AddAttribute( 7, nameof( SvgLineSeries<TooltipSample>.Value ), (Func<TooltipSample, double?>)( item => item.First ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgLineSeries<TooltipSample>>( 8 );
                builder.AddAttribute( 9, nameof( SvgLineSeries<TooltipSample>.Name ), "Second" );
                builder.AddAttribute( 10, nameof( SvgLineSeries<TooltipSample>.Value ), (Func<TooltipSample, double?>)( item => item.Second ) );
                builder.CloseComponent();
            } ) );
    }

    private sealed class TooltipSample
    {
        public string Category { get; set; }

        public double First { get; set; }

        public double Second { get; set; }
    }
}