#region Using directives
using System;
using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using Blazorise.Charts.Svg;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartLayoutComponentTest : BunitContext
{
    public SvgChartLayoutComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseSvgChart();
    }

    [Theory]
    [InlineData( true, true, 0, 24, 30, 306 )]
    [InlineData( true, false, 0, 36, 40, 284 )]
    [InlineData( false, true, 0, 24, 30, 354.5 )]
    [InlineData( true, true, 45, 24, 30, 306 )]
    public void BottomAxis_UsesVisibleLabelsAndActualRotation( bool visible, bool autoSkip, double maxRotation, double offset, double fontSize, double expectedBottom )
    {
        var component = RenderLineChart( new()
        {
            Legend = new() { Visible = false },
            XAxis = new()
            {
                Labels = new()
                {
                    Visible = visible,
                    AutoSkip = autoSkip,
                    MaxRotation = maxRotation,
                    Font = new() { Size = fontSize },
                    Offset = offset,
                },
            },
        } );

        var plot = component.Find( "clipPath rect" );
        Assert.Equal( expectedBottom, GetNumericAttribute( plot, "y" ) + GetNumericAttribute( plot, "height" ), 3 );
        Assert.All( component.FindAll( ".svg-chart-category-axis-labels text" ), label => Assert.Null( label.GetAttribute( "transform" ) ) );
    }

    [Fact]
    public void RotatedBottomLabels_ReserveTheirRotatedHeight()
    {
        var component = RenderLineChart( new()
        {
            Width = 200,
            Legend = new() { Visible = false },
            XAxis = new()
            {
                TickFormatter = _ => new string( 'x', 20 ),
                Labels = new() { MaxRotation = 50 },
            },
        } );

        var label = component.Find( ".svg-chart-category-axis-labels text" );
        var plot = component.Find( "clipPath rect" );
        var rotation = 50 * Math.PI / 180;
        var expectedHeight = 24 + 20 * 11 * 0.58 * Math.Sin( rotation ) + 11 * Math.Cos( rotation );

        Assert.StartsWith( "rotate(-50 ", label.GetAttribute( "transform" ) );
        Assert.Equal( expectedHeight, 360 - GetNumericAttribute( plot, "y" ) - GetNumericAttribute( plot, "height" ), 3 );
    }

    [Fact]
    public void BarAxis_UsesTruncatedTextAndCustomMeasurementWithoutMinimumWidth()
    {
        var component = Render<SvgBarChart<double?>>( parameters => parameters
            .Add( chart => chart.Options, new SvgChartOptions
            {
                Legend = new() { Visible = false },
                TextMeasurement = new()
                {
                    Measure = ( text, font ) => text.Sum( character => character == 'W' ? 50d : 3d ),
                },
                XAxis = new()
                {
                    TickFormatter = _ => "iiiW",
                    Labels = new()
                    {
                        MaxWidth = 12,
                        Offset = 2,
                    },
                },
            } )
            .AddChildContent<SvgBarSeries<double?>>( series => series.Add( item => item.Values, new double?[] { 1, 2, 3 } ) ) );

        var label = component.Find( ".svg-chart-grid > text[text-anchor='end']" );
        Assert.Equal( "iii", label.TextContent );
        Assert.Equal( "central", label.GetAttribute( "dominant-baseline" ) );
        Assert.Equal( 11.5, GetNumericAttribute( component.Find( "clipPath rect" ), "x" ), 3 );
    }

    [Theory]
    [InlineData( null, 71 )]
    [InlineData( 200d, 125 )]
    public void RightAxes_ReserveSeparateLabelAndTitleAreas( double? endPadding, double expectedDistance )
    {
        var component = Render<SvgLineChart<double?>>( parameters => parameters
            .Add( chart => chart.Options, new SvgChartOptions
            {
                Legend = new() { Visible = false },
                Layout = new() { AxisSpacing = 12 },
                PlotAreaPadding = new() { End = endPadding },
                TextMeasurement = new() { Measure = ( text, font ) => text.Length * 10 },
            } )
            .AddChildContent( builder =>
            {
                foreach ( var name in new[] { "primary", "first", "second" } )
                {
                    builder.OpenComponent<SvgLineSeries<double?>>( 0 );
                    builder.AddAttribute( 1, nameof( SvgLineSeries<double?>.Values ), new double?[] { 1, 2, 3 } );
                    builder.AddAttribute( 2, nameof( SvgLineSeries<double?>.ValueAxisId ), name );
                    builder.CloseComponent();

                    builder.OpenComponent<SvgChartValueAxis>( 3 );
                    builder.AddAttribute( 4, nameof( SvgChartValueAxis.Id ), name );
                    builder.AddAttribute( 5, nameof( SvgChartValueAxis.Position ), name == "primary" ? SvgChartAxisPosition.Left : SvgChartAxisPosition.Right );
                    builder.AddAttribute( 6, nameof( SvgChartValueAxis.Title ), (SvgChartAxisTitleOptions)name );
                    builder.AddAttribute( 7, nameof( SvgChartValueAxis.TickFormatter ), (Func<SvgChartAxisTickContext, string>)( _ => "100" ) );
                    builder.CloseComponent();
                }
            } ) );

        component.WaitForAssertion( () =>
        {
            var axes = component.FindAll( ".svg-chart-value-axis-right" );
            Assert.Equal( 2, axes.Count );
            Assert.Equal( expectedDistance, GetNumericAttribute( axes[1].QuerySelector( "line" ), "x1" ) - GetNumericAttribute( axes[0].QuerySelector( "line" ), "x1" ), 3 );
            Assert.Equal( 630.5, GetNumericAttribute( axes[1].QuerySelector( ".svg-chart-axis-title" ), "x" ), 3 );
        } );
    }

    [Theory]
    [InlineData( SvgChartLegendPosition.Top, SvgChartTextPosition.Top, 73, 120 )]
    [InlineData( SvgChartLegendPosition.Bottom, SvgChartTextPosition.Bottom, 285, 248 )]
    public void Legend_UsesFontSizePaddingAndTitlePosition( SvgChartLegendPosition position, SvgChartTextPosition textPosition, double expectedLegendY, double expectedPlotEdge )
    {
        var component = RenderLineChart( new()
        {
            Title = new()
            {
                Text = "Title",
                Position = textPosition,
                Font = new() { Size = 40 },
                Padding = new() { Top = 5, Bottom = 7 },
            },
            Legend = new()
            {
                Position = position,
                Padding = new() { Top = 5, Bottom = 7 },
                Labels = new()
                {
                    Font = new() { Size = 32 },
                    BoxWidth = 20,
                    BoxHeight = 20,
                },
            },
            XAxis = new() { Labels = new() { Visible = false } },
            PlotAreaPadding = new() { Bottom = 16 },
        }, legendComponent: true );

        var legend = component.Find( ".svg-chart-legend text" );
        var plot = component.Find( "clipPath rect" );
        Assert.Equal( "32", legend.GetAttribute( "font-size" ) );
        Assert.Equal( expectedLegendY, GetNumericAttribute( legend, "y" ), 3 );
        Assert.Equal( expectedPlotEdge, position == SvgChartLegendPosition.Top ? GetNumericAttribute( plot, "y" ) : GetNumericAttribute( plot, "y" ) + GetNumericAttribute( plot, "height" ), 3 );
    }

    [Theory]
    [InlineData( null, 7 )]
    [InlineData( 0d, 0 )]
    public void LegendLabels_InheritOptionsAndApplyPaddingToItemsAndRows( double? padding, double expectedPadding )
    {
        var component = RenderLineChart( new()
        {
            Width = 90,
            TextMeasurement = new() { Measure = ( text, font ) => 10 },
            Legend = new()
            {
                Position = SvgChartLegendPosition.Top,
                Padding = new(),
                Labels = new()
                {
                    Font = new() { Size = 12 },
                    BoxWidth = 20,
                    BoxHeight = 18,
                    Padding = 7,
                    LabelGap = 3,
                },
            },
        }, legendComponent: true, legendLabels: new()
        {
            Font = new() { Weight = "700" },
            Padding = padding,
        }, seriesCount: 3 );

        component.WaitForAssertion( () =>
        {
            var items = component.FindAll( ".svg-chart-legend-item" );
            Assert.Equal( 3, items.Count );
            Assert.All( items, item =>
            {
                Assert.Equal( "20", item.QuerySelector( "rect" ).GetAttribute( "width" ) );
                Assert.Equal( "18", item.QuerySelector( "rect" ).GetAttribute( "height" ) );
                Assert.Equal( "12", item.QuerySelector( "text" ).GetAttribute( "font-size" ) );
                Assert.Equal( "700", item.QuerySelector( "text" ).GetAttribute( "font-weight" ) );
            } );

            Assert.Equal( 33 + expectedPadding, GetNumericAttribute( items[1].QuerySelector( "rect" ), "x" ) - GetNumericAttribute( items[0].QuerySelector( "rect" ), "x" ), 3 );
            Assert.Equal( 18 + expectedPadding, GetNumericAttribute( items[2].QuerySelector( "text" ), "y" ) - GetNumericAttribute( items[0].QuerySelector( "text" ), "y" ), 3 );
            Assert.Equal( 60 + expectedPadding, GetNumericAttribute( component.Find( "clipPath rect" ), "y" ), 3 );
        } );
    }

    private IRenderedComponent<SvgLineChart<double?>> RenderLineChart( SvgChartOptions options, bool legendComponent = false, SvgChartLegendLabelsOptions legendLabels = null, int seriesCount = 1 )
    {
        return Render<SvgLineChart<double?>>( parameters => parameters
            .Add( chart => chart.Options, options )
            .AddChildContent( builder =>
            {
                for ( var i = 0; i < seriesCount; i++ )
                {
                    builder.OpenComponent<SvgLineSeries<double?>>( 0 );
                    builder.AddAttribute( 1, nameof( SvgLineSeries<double?>.Name ), $"Series {i + 1}" );
                    builder.AddAttribute( 2, nameof( SvgLineSeries<double?>.Values ), new double?[] { 1, 2, 3 } );
                    builder.CloseComponent();
                }

                if ( legendComponent )
                {
                    builder.OpenComponent<SvgChartLegend>( 3 );

                    if ( legendLabels is not null )
                    {
                        builder.AddAttribute( 4, nameof( SvgChartLegend.Labels ), legendLabels );
                    }

                    builder.CloseComponent();
                }
            } ) );
    }

    private static double GetNumericAttribute( IElement element, string attribute ) => double.Parse( element.GetAttribute( attribute ), CultureInfo.InvariantCulture );
}