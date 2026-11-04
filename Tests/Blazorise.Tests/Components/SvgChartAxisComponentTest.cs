#region Using directives
using System;
using System.Globalization;
using AngleSharp.Dom;
using Blazorise.Charts.Svg;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartAxisComponentTest : BunitContext
{
    public SvgChartAxisComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseSvgChart();
    }

    [Theory]
    [InlineData( 11, null, "$30,000K", 60, 80 )]
    [InlineData( 22, null, "$30,000K", 110, 130 )]
    [InlineData( 22, 40d, "$30", 45, 60 )]
    public void RightAxisTitle_ReservesSpaceForFormattedLabels( double fontSize, double? maxWidth, string expectedLabel, double minimumTitleOffset, double maximumTitleOffset )
    {
        var component = RenderChart( new(), new()
        {
            Font = new() { Size = fontSize },
            MaxWidth = maxWidth,
        } );

        component.WaitForAssertion( () =>
        {
            var axis = component.Find( ".svg-chart-value-axis-right" );
            var label = axis.QuerySelector( "text:not(.svg-chart-axis-title)" );
            var title = axis.QuerySelector( ".svg-chart-axis-title" );
            var axisX = GetNumericAttribute( axis.QuerySelector( "line" ), "x1" );
            var labelX = GetNumericAttribute( label, "x" );
            var titleX = GetNumericAttribute( title, "x" );
            var plot = component.Find( "clipPath rect" );
            var plotRight = GetNumericAttribute( plot, "x" ) + GetNumericAttribute( plot, "width" );

            Assert.Contains( axis.QuerySelectorAll( "text:not(.svg-chart-axis-title)" ), tick => tick.TextContent == expectedLabel );
            Assert.Equal( axisX + 10, labelX, 3 );
            Assert.Equal( plotRight, axisX, 3 );
            Assert.InRange( titleX - labelX, minimumTitleOffset, maximumTitleOffset );
            Assert.InRange( titleX, 620, 640 );
        } );
    }

    [Theory]
    [InlineData( null, false, true, 621 )]
    [InlineData( 90d, true, true, 531 )]
    [InlineData( 18d, true, true, 603 )]
    [InlineData( null, true, false, 622 )]
    public void EndPadding_OverridesAutomaticLabelSpacing( double? endPadding, bool labelsVisible, bool secondaryAxis, double expectedPlotRight )
    {
        var component = RenderChart( new()
        {
            PlotAreaPadding = new() { End = endPadding },
        }, new()
        {
            Visible = labelsVisible,
        }, secondaryAxis );

        component.WaitForAssertion( () =>
        {
            var plot = component.Find( "clipPath rect" );
            var plotRight = GetNumericAttribute( plot, "x" ) + GetNumericAttribute( plot, "width" );

            Assert.Equal( expectedPlotRight, plotRight, 3 );
        } );
    }

    [Theory]
    [InlineData( "120", 11, null, "120", 47, 50 )]
    [InlineData( "$30,000K", 11, null, "$30,000K", 79, 82 )]
    [InlineData( "$30,000K", 22, null, "$30,000K", 130, 133 )]
    [InlineData( "$30,000K", 22, 40d, "$30", 66, 69 )]
    public void LeftAxisTitle_ReservesSpaceForFormattedLabels( string formattedLabel, double fontSize, double? maxWidth, string expectedLabel, double minimumPlotLeft, double maximumPlotLeft )
    {
        var component = RenderChart( new()
        {
            YAxis = new()
            {
                Title = "Revenue",
                TickFormatter = context => formattedLabel,
                Labels = new()
                {
                    Font = new() { Size = fontSize },
                    MaxWidth = maxWidth,
                },
            },
        }, null, secondaryAxis: false );

        component.WaitForAssertion( () =>
        {
            var label = component.Find( ".svg-chart-grid > text:not(.svg-chart-axis-title)" );
            var title = component.Find( ".svg-chart-grid > .svg-chart-value-axis-title" );
            var plotLeft = GetNumericAttribute( component.Find( "clipPath rect" ), "x" ) - 0.5;
            var labelX = GetNumericAttribute( label, "x" );
            var titleX = GetNumericAttribute( title, "x" );

            Assert.Equal( expectedLabel, label.TextContent );
            Assert.Equal( plotLeft - 10, labelX, 3 );
            Assert.InRange( plotLeft, minimumPlotLeft, maximumPlotLeft );
            Assert.InRange( titleX, 0, 19 );
        } );
    }

    [Theory]
    [InlineData( null, false, 19 )]
    [InlineData( 90d, true, 109 )]
    [InlineData( 18d, true, 37 )]
    public void StartPadding_PreservesExplicitPaddingAndSkipsHiddenLabels( double? startPadding, bool labelsVisible, double expectedPlotLeft )
    {
        var component = RenderChart( new()
        {
            PlotAreaPadding = new() { Start = startPadding },
            YAxis = new()
            {
                Title = "Revenue",
                TickFormatter = context => "$30,000K",
                Labels = new() { Visible = labelsVisible },
            },
        }, null, secondaryAxis: false );

        component.WaitForAssertion( () =>
        {
            var plotLeft = GetNumericAttribute( component.Find( "clipPath rect" ), "x" ) - 0.5;

            Assert.Equal( expectedPlotLeft, plotLeft, 3 );
        } );
    }

    [Theory]
    [InlineData( null, null, 16, 14 )]
    [InlineData( 0d, 0d, 0, 0 )]
    [InlineData( 24d, 20d, 24, 20 )]
    public void RightAxis_InheritsSpacingAndHonorsComponentOverrides( double? labelOffset, double? titlePadding, double expectedLabelOffset, double expectedTitlePadding )
    {
        var labels = new SvgChartAxisLabelsOptions
        {
            Font = new() { Size = 11 },
            Offset = labelOffset,
        };

        var component = RenderChart( new()
        {
            YAxis = new()
            {
                Title = new() { Padding = new() { Top = 14, Bottom = 14 } },
                Labels = new() { Offset = 16 },
            },
        }, labels, rightTitle: new()
        {
            Text = "Budget",
            Padding = titlePadding.HasValue ? new() { Top = titlePadding.Value, Bottom = titlePadding.Value } : null,
        } );

        component.WaitForAssertion( () =>
        {
            var axis = component.Find( ".svg-chart-value-axis-right" );
            var axisX = GetNumericAttribute( axis.QuerySelector( "line" ), "x1" );
            var labelX = GetNumericAttribute( axis.QuerySelector( "text:not(.svg-chart-axis-title)" ), "x" );
            var titleX = GetNumericAttribute( axis.QuerySelector( ".svg-chart-axis-title" ), "x" );
            var plot = component.Find( "clipPath rect" );
            var plotLeft = GetNumericAttribute( plot, "x" ) - 0.5;
            var primaryLabelX = GetNumericAttribute( component.Find( ".svg-chart-grid > text:not(.svg-chart-axis-title)" ), "x" );

            Assert.Equal( expectedLabelOffset, labelX - axisX, 3 );
            Assert.Equal( 16, plotLeft - primaryLabelX, 3 );
            Assert.Equal( 640 - expectedTitlePadding - 5.5, titleX, 3 );
            Assert.Equal( 51.04 + expectedTitlePadding, titleX - 5.5 - labelX, 3 );
        } );
    }

    [Theory]
    [InlineData( null, null, null, 24, 4, 4 )]
    [InlineData( 36d, null, null, 36, 4, 4 )]
    [InlineData( 0d, 0d, 0d, 0, 0, 0 )]
    [InlineData( 24d, 3d, 17d, 24, 3, 17 )]
    public void CategoryAxis_ResolvesHorizontalOffsetAndTitlePadding( double? labelOffset, double? topPadding, double? bottomPadding, double expectedLabelOffset, double expectedTopPadding, double expectedBottomPadding )
    {
        var options = new SvgChartOptions
        {
            XAxis = new()
            {
                Title = "Month",
                Labels = new()
                {
                    AutoRotate = false,
                    Offset = labelOffset,
                },
            },
            PlotAreaPadding = new() { Bottom = 50 },
            Legend = new() { Visible = false },
        };

        var component = RenderChart( options, null, secondaryAxis: false, categoryTitle: topPadding.HasValue ? new()
        {
            Padding = new() { Top = topPadding.Value, Bottom = bottomPadding.Value },
        } : null );

        component.WaitForAssertion( () =>
        {
            var plot = component.Find( "clipPath rect" );
            var plotBottom = GetNumericAttribute( plot, "y" ) + GetNumericAttribute( plot, "height" );
            var labelY = GetNumericAttribute( component.Find( ".svg-chart-category-axis-labels text" ), "y" );
            var title = component.Find( ".svg-chart-category-axis-title" );
            var titleY = GetNumericAttribute( title, "y" );

            Assert.Equal( expectedLabelOffset, labelY - plotBottom, 3 );
            Assert.Equal( expectedTopPadding, titleY - 5.5 - plotBottom - 50, 3 );
            Assert.Equal( 299 - expectedTopPadding - expectedBottomPadding, plotBottom, 3 );
            Assert.Equal( "Month", title.TextContent );
            Assert.Equal( "central", title.GetAttribute( "dominant-baseline" ) );
        } );
    }

    [Fact]
    public void BarChart_AppliesSpacingToTheCorrectAxisOrientation()
    {
        var component = Render<SvgBarChart<double?>>( parameters => parameters
            .Add( chart => chart.Options, new SvgChartOptions
            {
                YAxis = new()
                {
                    Title = new()
                    {
                        Text = "Quantity",
                        Padding = new() { Top = 20, Bottom = 20 },
                    },
                },
            } )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgBarSeries<double?>>( 0 );
                builder.AddAttribute( 1, nameof( SvgBarSeries<double?>.Values ), new double?[] { 1, 2, 3 } );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<double?>>( 2 );
                builder.AddAttribute( 3, nameof( SvgChartCategoryAxis<double?>.Title ), new SvgChartAxisTitleOptions
                {
                    Text = "Category",
                    Padding = new() { Top = 12, Bottom = 12 },
                } );
                builder.AddAttribute( 5, nameof( SvgChartCategoryAxis<double?>.LabelsOptions ), new SvgChartAxisLabelsOptions { Offset = 30 } );
                builder.CloseComponent();
            } ) );

        component.WaitForAssertion( () =>
        {
            var plot = component.Find( "clipPath rect" );
            var plotLeft = GetNumericAttribute( plot, "x" ) - 0.5;
            var plotBottom = GetNumericAttribute( plot, "y" ) + GetNumericAttribute( plot, "height" );
            var categoryLabelX = GetNumericAttribute( component.Find( ".svg-chart-grid > text[text-anchor='end']" ), "x" );
            var valueLabelY = GetNumericAttribute( component.Find( ".svg-chart-grid > text[text-anchor='middle']:not(.svg-chart-axis-title)" ), "y" );
            var categoryTitleX = GetNumericAttribute( component.Find( ".svg-chart-category-axis-title" ), "x" );
            var valueTitleY = GetNumericAttribute( component.Find( ".svg-chart-value-axis-title" ), "y" );

            Assert.Equal( 30, plotLeft - categoryLabelX, 3 );
            Assert.Equal( 24, valueLabelY - plotBottom, 3 );
            Assert.Equal( 17.5, categoryTitleX, 3 );
            Assert.Equal( 60.5, valueTitleY - plotBottom, 3 );
        } );
    }

    [Fact]
    public void RightAxisTitle_UsesAsymmetricPaddingAndItsOwnFont()
    {
        var component = RenderChart( new(), null, rightTitle: new()
        {
            Text = "Budget",
            Padding = new() { Top = 3, Bottom = 11 },
            Font = new()
            {
                Size = 22,
                Weight = "700",
            },
        } );

        component.WaitForAssertion( () =>
        {
            var axis = component.Find( ".svg-chart-value-axis-right" );
            var labelX = GetNumericAttribute( axis.QuerySelector( "text:not(.svg-chart-axis-title)" ), "x" );
            var title = axis.QuerySelector( ".svg-chart-axis-title" );
            var titleX = GetNumericAttribute( title, "x" );

            Assert.Equal( "22", title.GetAttribute( "font-size" ) );
            Assert.Equal( "700", title.GetAttribute( "font-weight" ) );
            Assert.Equal( 626, titleX, 3 );
            Assert.Equal( 62.04, titleX - 11 - labelX, 3 );
        } );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    public void EmptyStringTitle_DoesNotRenderOrReserveTitleSpace( string title )
    {
        var component = RenderChart( new()
        {
            YAxis = new()
            {
                Title = title,
                Labels = new() { Visible = false },
            },
        }, null, secondaryAxis: false );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( ".svg-chart-value-axis-title" ) );
            Assert.Equal( "0.5", component.Find( "clipPath rect" ).GetAttribute( "x" ) );
        } );
    }

    private static double GetNumericAttribute( IElement element, string attribute ) => double.Parse( element.GetAttribute( attribute ), CultureInfo.InvariantCulture );

    private IRenderedComponent<SvgLineChart<double?>> RenderChart( SvgChartOptions options, SvgChartAxisLabelsOptions labels, bool secondaryAxis = true, SvgChartAxisTitleOptions rightTitle = null, SvgChartAxisTitleOptions categoryTitle = null )
    {
        return Render<SvgLineChart<double?>>( parameters => parameters
            .Add( chart => chart.Options, options )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgLineSeries<double?>>( 0 );
                builder.AddAttribute( 1, nameof( SvgLineSeries<double?>.Values ), new double?[] { 1, 2, 3 } );
                builder.AddAttribute( 2, nameof( SvgLineSeries<double?>.ValueAxisId ), "primary" );
                builder.CloseComponent();

                if ( secondaryAxis )
                {
                    builder.OpenComponent<SvgLineSeries<double?>>( 3 );
                    builder.AddAttribute( 4, nameof( SvgLineSeries<double?>.Values ), new double?[] { 10000, 20000, 30000 } );
                    builder.AddAttribute( 5, nameof( SvgLineSeries<double?>.ValueAxisId ), "budget" );
                    builder.CloseComponent();

                    builder.OpenComponent<SvgChartValueAxis>( 6 );
                    builder.AddAttribute( 7, nameof( SvgChartValueAxis.Id ), "budget" );
                    builder.AddAttribute( 8, nameof( SvgChartValueAxis.Position ), SvgChartAxisPosition.Right );
                    builder.AddAttribute( 9, nameof( SvgChartValueAxis.Title ), rightTitle ?? (SvgChartAxisTitleOptions)"Budget" );
                    builder.AddAttribute( 10, nameof( SvgChartValueAxis.Min ), 0d );
                    builder.AddAttribute( 11, nameof( SvgChartValueAxis.Max ), 30000d );
                    builder.AddAttribute( 12, nameof( SvgChartValueAxis.LabelsOptions ), labels );
                    builder.AddAttribute( 13, nameof( SvgChartValueAxis.TickFormatter ), (Func<SvgChartAxisTickContext, string>)( context => $"${Convert.ToDouble( context.Value, CultureInfo.InvariantCulture ).ToString( "N0", CultureInfo.InvariantCulture )}K" ) );

                    builder.CloseComponent();
                }

                builder.OpenComponent<SvgChartValueAxis>( 15 );
                builder.AddAttribute( 16, nameof( SvgChartValueAxis.Id ), "primary" );
                builder.CloseComponent();

                if ( categoryTitle is not null )
                {
                    builder.OpenComponent<SvgChartCategoryAxis<double?>>( 17 );
                    builder.AddAttribute( 18, nameof( SvgChartCategoryAxis<double?>.Title ), categoryTitle );
                    builder.CloseComponent();
                }
            } ) );
    }
}