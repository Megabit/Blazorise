using System.Collections.Generic;
using Blazorise.Charts.Svg;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class SvgLineSeriesComponentTest : BunitContext
{
    public SvgLineSeriesComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void Outline_RendersBehindLineWithoutDuplicatingSeriesElements()
    {
        SvgChartLineOutlineOptions outline = new()
        {
            Color = "rgba(255, 215, 0, 0.5)",
            StrokeWidth = 8,
            Opacity = 0.35
        };

        IRenderedComponent<SvgLineChart<double?>> component = RenderLineChart( outline );

        component.WaitForAssertion( () =>
        {
            var paths = component.FindAll( ".svg-chart-lines > path" );
            var outlinePath = component.Find( ".svg-chart-line-outline" );

            Assert.Equal( 2, paths.Count );
            Assert.Equal( "svg-chart-line-outline", paths[0].ClassName );
            Assert.Equal( "svg-chart-line", paths[1].ClassName );
            Assert.Equal( "rgba(255, 215, 0, 0.5)", outlinePath.GetAttribute( "stroke" ) );
            Assert.Equal( "8", outlinePath.GetAttribute( "stroke-width" ) );
            Assert.Equal( "0.35", outlinePath.GetAttribute( "stroke-opacity" ) );
            Assert.Equal( "none", outlinePath.GetAttribute( "pointer-events" ) );
            Assert.Equal( 3, component.FindAll( ".svg-chart-marker" ).Count );
            Assert.Single( component.FindAll( ".svg-chart-legend-item" ) );
        } );
    }

    [Fact]
    public void Outline_UsesSeriesColorWhenColorIsOmitted()
    {
        SvgChartLineOutlineOptions outline = new()
        {
            StrokeWidth = 8
        };

        IRenderedComponent<SvgLineChart<double?>> component = RenderLineChart( outline );

        component.WaitForAssertion( () =>
        {
            string lineColor = component.Find( ".svg-chart-line" ).GetAttribute( "stroke" );
            string outlineColor = component.Find( ".svg-chart-line-outline" ).GetAttribute( "stroke" );

            Assert.Equal( lineColor, outlineColor );
        } );
    }

    private IRenderedComponent<SvgLineChart<double?>> RenderLineChart( SvgChartLineOutlineOptions outline )
    {
        List<double?> values = [1, 2, 3];

        return Render<SvgLineChart<double?>>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgLineSeries<double?>>( 0 );
                builder.AddAttribute( 1, nameof( SvgLineSeries<double?>.Name ), "Average" );
                builder.AddAttribute( 2, nameof( SvgLineSeries<double?>.Values ), values );
                builder.AddAttribute( 3, nameof( SvgLineSeries<double?>.Color ), Color.Danger );
                builder.AddAttribute( 4, nameof( SvgLineSeries<double?>.Outline ), outline );
                builder.CloseComponent();
            } ) );
    }
}