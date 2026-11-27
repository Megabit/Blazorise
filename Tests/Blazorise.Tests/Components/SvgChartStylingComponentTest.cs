#region Using directives
using System;
using Blazorise.Charts.Svg;
using Blazorise.Utilities;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartStylingComponentTest : BunitContext
{
    public SvgChartStylingComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseSvgChart();
    }

    [Fact]
    public void SeriesStyling_UpdatesAndClearsTheRenderedGroup()
    {
        var className = "revenue";
        var style = "opacity:0.5";

        var component = Render<SvgLineChart<double?>>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgLineSeries<double?>>( 0 );
                builder.AddAttribute( 1, nameof( SvgLineSeries<double?>.Values ), new double?[] { 1, 2, 3 } );
                builder.AddAttribute( 2, nameof( SvgLineSeries<double?>.Class ), className );
                builder.AddAttribute( 3, nameof( SvgLineSeries<double?>.Style ), style );
                builder.CloseComponent();
            } ) );

        Assert.Contains( "opacity:0.5", component.Find( ".svg-chart-line-series.revenue" ).GetAttribute( "style" ) );

        className = "expenses";
        style = "opacity:0.8";
        component.Render();

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( ".revenue" ) );
            Assert.Contains( "opacity:0.8", component.Find( ".svg-chart-line-series.expenses" ).GetAttribute( "style" ) );
        } );

        className = null;
        style = null;
        component.Render();

        component.WaitForAssertion( () =>
        {
            var group = component.Find( ".svg-chart-line-series" );

            Assert.Equal( "svg-chart-line-series", group.GetAttribute( "class" ) );
            Assert.True( string.IsNullOrEmpty( group.GetAttribute( "style" ) ) );
        } );
    }

    [Fact]
    public void ChartParts_UseGeneratedStylesAndClearCustomStyling()
    {
        var customClass = "custom";
        var style = "opacity:0.6";

        var component = RenderChart( SvgChartType.Line, builder =>
        {
            builder.OpenComponent<SvgChartTitle>( 0 );
            builder.AddAttribute( 1, nameof( SvgChartTitle.Title ), new SvgChartTextOptions { Text = "Title" } );
            builder.AddAttribute( 2, nameof( SvgChartTitle.Subtitle ), new SvgChartTextOptions { Text = "Subtitle" } );
            builder.AddAttribute( 3, nameof( SvgChartTitle.Class ), customClass );
            builder.AddAttribute( 4, nameof( SvgChartTitle.Style ), style );
            builder.CloseComponent();

            builder.OpenComponent<StyledLegend>( 5 );
            builder.AddAttribute( 6, nameof( SvgChartLegend.Class ), customClass );
            builder.AddAttribute( 7, nameof( SvgChartLegend.Style ), style );
            builder.CloseComponent();

            builder.OpenComponent<SvgChartTooltip>( 8 );
            builder.AddAttribute( 9, nameof( SvgChartTooltip.Intersect ), false );
            builder.AddAttribute( 10, nameof( SvgChartTooltip.Class ), customClass );
            builder.AddAttribute( 11, nameof( SvgChartTooltip.Style ), style );
            builder.CloseComponent();
        } );

        Assert.Equal( "Title", component.Find( ".custom .svg-chart-title" ).TextContent );
        Assert.Contains( "opacity:0.6", component.Find( ".svg-chart-legend.custom.generated-legend" ).GetAttribute( "style" ) );
        Assert.Contains( "--legend-accent:1", component.Find( ".svg-chart-legend" ).GetAttribute( "style" ) );

        component.Find( ".svg-chart-tooltip-hit-region" ).MouseEnter();

        var tooltip = component.Find( ".svg-chart-tooltip.custom" );

        Assert.Contains( "position:absolute", tooltip.GetAttribute( "style" ) );
        Assert.Contains( "opacity:0.6", tooltip.GetAttribute( "style" ) );

        customClass = null;
        style = null;
        component.Render();

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( ".custom" ) );
            Assert.Contains( "--legend-accent:1", component.Find( ".svg-chart-legend.generated-legend" ).GetAttribute( "style" ) );
            Assert.DoesNotContain( "opacity:0.6", component.Find( ".svg-chart-tooltip" ).GetAttribute( "style" ) );
        } );
    }

    [Theory]
    [InlineData( SvgChartType.Line, false )]
    [InlineData( SvgChartType.Bar, false )]
    [InlineData( SvgChartType.Scatter, false )]
    [InlineData( SvgChartType.Line, true )]
    public void AxisStyling_IsScopedToItsOwnGridLabelsAndTitle( SvgChartType type, bool timeAxis )
    {
        var component = RenderChart( type, builder =>
        {
            builder.OpenComponent( 0, timeAxis ? typeof( SvgChartTimeAxis<double?> ) : typeof( SvgChartCategoryAxis<double?> ) );
            builder.AddAttribute( 1, nameof( SvgChartCategoryAxis<double?>.Class ), "styled-category" );
            builder.AddAttribute( 2, nameof( SvgChartCategoryAxis<double?>.Style ), "opacity:0.5" );
            builder.AddAttribute( 3, nameof( SvgChartCategoryAxis<double?>.Title ), (SvgChartAxisTitleOptions)"Category" );
            builder.AddAttribute( 4, nameof( SvgChartCategoryAxis<double?>.GridLines ), new SvgChartGridLinesOptions { Visible = true } );

            if ( timeAxis )
            {
                builder.AddAttribute( 5, nameof( SvgChartTimeAxis<double?>.Scale ), SvgChartTimeScale.Continuous );
            }

            builder.CloseComponent();

            builder.OpenComponent<SvgChartValueAxis>( 6 );
            builder.AddAttribute( 7, nameof( SvgChartValueAxis.Id ), "primary" );
            builder.AddAttribute( 8, nameof( SvgChartValueAxis.Class ), "styled-value" );
            builder.AddAttribute( 9, nameof( SvgChartValueAxis.Style ), "opacity:0.7" );
            builder.AddAttribute( 10, nameof( SvgChartValueAxis.Title ), (SvgChartAxisTitleOptions)"Value" );
            builder.CloseComponent();

            builder.OpenComponent<SvgChartValueAxis>( 11 );
            builder.AddAttribute( 12, nameof( SvgChartValueAxis.Id ), "secondary" );
            builder.AddAttribute( 13, nameof( SvgChartValueAxis.Position ), SvgChartAxisPosition.Right );
            builder.AddAttribute( 14, nameof( SvgChartValueAxis.Class ), "styled-secondary" );
            builder.AddAttribute( 15, nameof( SvgChartValueAxis.Style ), "opacity:0.9" );
            builder.AddAttribute( 16, nameof( SvgChartValueAxis.Title ), (SvgChartAxisTitleOptions)"Secondary" );
            builder.CloseComponent();
        }, timeAxis );

        Assert.Equal( "Category", component.Find( ".styled-category .svg-chart-category-axis-title" ).TextContent );
        Assert.Equal( "Value", component.Find( ".styled-value .svg-chart-value-axis-title" ).TextContent );
        Assert.NotEmpty( component.FindAll( ".styled-value line" ) );
        Assert.All( component.FindAll( ".styled-category" ), element => Assert.Contains( "opacity:0.5", element.GetAttribute( "style" ) ) );
        Assert.All( component.FindAll( ".styled-value" ), element => Assert.Contains( "opacity:0.7", element.GetAttribute( "style" ) ) );
        Assert.Empty( component.FindAll( ".styled-value .styled-category, .styled-category .styled-value, .styled-value .styled-secondary" ) );

        if ( type != SvgChartType.Bar )
        {
            var gridClass = timeAxis || type == SvgChartType.Scatter ? "svg-chart-point-xaxis-grid" : "svg-chart-category-grid";

            Assert.NotEmpty( component.FindAll( $".{gridClass}.styled-category line" ) );
            Assert.Equal( "Secondary", component.Find( ".svg-chart-value-axis-right.styled-secondary .svg-chart-axis-title" ).TextContent );
            Assert.Contains( "opacity:0.9", component.Find( ".styled-secondary" ).GetAttribute( "style" ) );
        }
    }

    [Fact]
    public void VisualPlugins_PreserveStylingThroughDataLabelCloningAndIncludeAnnotationLabels()
    {
        var component = RenderChart( SvgChartType.Line, builder =>
        {
            builder.OpenComponent<SvgChartDataLabels>( 0 );
            builder.AddAttribute( 1, nameof( SvgChartDataLabels.Visible ), true );
            builder.AddAttribute( 2, nameof( SvgChartDataLabels.Class ), "styled-labels" );
            builder.AddAttribute( 3, nameof( SvgChartDataLabels.Style ), "opacity:0.5" );
            builder.CloseComponent();

            builder.OpenComponent<SvgChartBoxAnnotation>( 4 );
            builder.AddAttribute( 5, nameof( SvgChartBoxAnnotation.Class ), "styled-annotation" );
            builder.AddAttribute( 6, nameof( SvgChartBoxAnnotation.Style ), "opacity:0.7" );
            builder.AddAttribute( 7, nameof( SvgChartBoxAnnotation.Label ), new SvgChartAnnotationLabelOptions { Visible = true, Text = "Band" } );
            builder.CloseComponent();

            builder.OpenComponent<SvgChartTrendline>( 8 );
            builder.AddAttribute( 9, nameof( SvgChartTrendline.SeriesName ), "Values" );
            builder.AddAttribute( 10, nameof( SvgChartTrendline.Class ), "styled-trendline" );
            builder.AddAttribute( 11, nameof( SvgChartTrendline.Style ), "opacity:0.9" );
            builder.CloseComponent();
        } );

        Assert.NotEmpty( component.FindAll( ".styled-labels .svg-chart-data-label" ) );
        Assert.Contains( "opacity:0.5", component.Find( ".styled-labels" ).GetAttribute( "style" ) );
        Assert.Single( component.FindAll( ".styled-annotation .svg-chart-box-annotation" ) );
        Assert.Single( component.FindAll( ".styled-annotation .svg-chart-annotation-label" ) );
        Assert.Contains( "opacity:0.7", component.Find( ".styled-annotation" ).GetAttribute( "style" ) );
        Assert.Single( component.FindAll( ".styled-trendline .svg-chart-trendline" ) );
        Assert.Contains( "opacity:0.9", component.Find( ".styled-trendline" ).GetAttribute( "style" ) );
    }

    [Fact]
    public void RadarValueAxisStyling_AppliesToGridWithoutStylingSeries()
    {
        var component = RenderChart( SvgChartType.Radar, builder =>
        {
            builder.OpenComponent<SvgChartValueAxis>( 0 );
            builder.AddAttribute( 1, nameof( SvgChartValueAxis.Id ), "primary" );
            builder.AddAttribute( 2, nameof( SvgChartValueAxis.Class ), "styled-radar-axis" );
            builder.AddAttribute( 3, nameof( SvgChartValueAxis.Style ), "opacity:0.5" );
            builder.CloseComponent();
        } );

        Assert.Equal( 4, component.FindAll( ".styled-radar-axis > polygon" ).Count );
        Assert.Contains( "opacity:0.5", component.Find( ".styled-radar-axis" ).GetAttribute( "style" ) );
        Assert.Empty( component.FindAll( ".styled-radar-axis .svg-chart-radar-series" ) );
    }

    [Fact]
    public void DataSeriesStyling_PreservesTheGroupClassesAndVisibility()
    {
        var data = new SvgChartData<double?>
        {
            Labels = ["A", "B", "C"],
            Series =
            [
                new()
                {
                    Name = "Revenue",
                    Class = "revenue",
                    Style = "opacity:0.5",
                    Values = [1, 2, 3],
                },
            ],
        };

        var component = Render<SvgLineChart<double?>>( parameters => parameters
            .Add( chart => chart.Data, data ) );

        Assert.Equal( "opacity:0.5", component.Find( ".svg-chart-line-series.revenue" ).GetAttribute( "style" ) );

        component.Find( ".svg-chart-legend-item" ).Click();

        Assert.Equal( "hidden", component.Find( ".svg-chart-line-series.revenue" ).GetAttribute( "visibility" ) );
        Assert.Equal( "opacity:0.5", component.Find( ".svg-chart-line-series.revenue" ).GetAttribute( "style" ) );
    }

    private IRenderedComponent<SvgChart<double?>> RenderChart( SvgChartType type, RenderFragment childContent, bool timeAxis = false )
    {
        var data = new SvgChartData<double?>
        {
            Labels = timeAxis
                ? [new DateTime( 2026, 1, 1 ), new DateTime( 2026, 1, 2 ), new DateTime( 2026, 1, 3 )]
                : ["A", "B", "C"],
            Series = [new() { Name = "Values", ValueAxisId = "primary", Values = [1, 2, 3] }],
        };

        return Render<SvgChart<double?>>( parameters => parameters
            .Add( chart => chart.Type, type )
            .Add( chart => chart.Data, data )
            .AddChildContent( childContent ) );
    }

    public sealed class StyledLegend : SvgChartLegend
    {
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "generated-legend" );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            builder.Append( "--legend-accent:1" );

            base.BuildStyles( builder );
        }
    }
}