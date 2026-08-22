#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Charts.Svg;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartDataDragComponentTest : BunitContext
{
    public SvgChartDataDragComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();

        var module = JSInterop.SetupModule( "./_content/Blazorise.Charts.Svg/svgChart.js" );
        module.SetupVoid( "initializeDataDrag", _ => true ).SetVoidResult();
        module.SetupVoid( "destroyDataDrag", _ => true ).SetVoidResult();
    }

    [Fact]
    public void ShowTooltip_DefaultsToFalse()
    {
        Assert.False( new SvgChartDataDrag().ShowTooltip );
        Assert.False( new SvgChartDataDragOptions().ShowTooltip );
    }

    [Fact]
    public void Enabled_RendersDraggableMarkersAndExpandedHitTargets()
    {
        var component = RenderLineChart();

        component.WaitForAssertion( () =>
        {
            Assert.Contains( "svg-chart-data-drag-enabled", component.Find( "svg" ).GetAttribute( "class" ) );
            Assert.Equal( 3, component.FindAll( ".svg-chart-marker[data-svg-chart-draggable='true']" ).Count );

            Assert.All( component.FindAll( ".svg-chart-marker[data-svg-chart-draggable='true']" ), marker =>
            {
                Assert.Equal( "ArrowLeft ArrowRight ArrowUp ArrowDown", marker.GetAttribute( "aria-keyshortcuts" ) );
                Assert.Contains( "Use arrow keys to adjust.", marker.GetAttribute( "aria-label" ) );
            } );

            var hitTargets = component.FindAll( ".svg-chart-data-drag-hit-target" );

            Assert.Equal( 3, hitTargets.Count );
            Assert.All( hitTargets, target => Assert.Equal( "18", target.GetAttribute( "r" ) ) );
        } );
    }

    [Fact]
    public void CanDrag_FiltersIndividualPoints()
    {
        var component = RenderLineChart( point => point.PointIndex == 1 );

        component.WaitForAssertion( () =>
        {
            var markers = component.FindAll( ".svg-chart-marker[data-svg-chart-draggable='true']" );

            Assert.Single( markers );
            Assert.Equal( "1", markers[0].GetAttribute( "data-svg-chart-point-index" ) );
        } );
    }

    [Fact]
    public void DraggableFalse_DisablesSeriesPoints()
    {
        var component = RenderLineChart( draggable: false );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( "[data-svg-chart-draggable='true']" ) );
            Assert.Empty( component.FindAll( ".svg-chart-data-drag-hit-target" ) );
        } );
    }

    [Fact]
    public void StreamingEnabled_DisablesDataPointDragging()
    {
        var options = new SvgChartOptions
        {
            Streaming = new() { Enabled = true }
        };
        var component = RenderLineChart( options: options );

        component.WaitForAssertion( () => Assert.Empty( component.FindAll( "[data-svg-chart-draggable='true']" ) ) );
    }

    [Fact]
    public void XMode_OnlyEnablesPointSeriesWithContinuousXValues()
    {
        List<DragSample> samples =
        [
            new() { X = 1, Y = 10 },
            new() { X = 2, Y = 20 }
        ];
        var options = new SvgChartOptions
        {
            DataDrag = new()
            {
                Enabled = true,
                Mode = SvgChartDataDragMode.X
            }
        };

        var component = Render<SvgScatterChart<DragSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .Add( chart => chart.Options, options )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgScatterSeries<DragSample>>( 0 );
                builder.AddAttribute( 1, nameof( SvgScatterSeries<DragSample>.Name ), "Samples" );
                builder.AddAttribute( 2, nameof( SvgScatterSeries<DragSample>.XValue ), (Func<DragSample, double?>)( item => item.X ) );
                builder.AddAttribute( 3, nameof( SvgScatterSeries<DragSample>.YValue ), (Func<DragSample, double?>)( item => item.Y ) );
                builder.CloseComponent();
            } ) );

        component.WaitForAssertion( () => Assert.Equal( 2, component.FindAll( ".svg-chart-scatter[data-svg-chart-draggable='true']" ).Count ) );
    }

    [Fact]
    public void XMode_EnablesHorizontalBars()
    {
        var component = RenderBarChart();

        component.WaitForAssertion( () =>
        {
            Assert.Equal( 3, component.FindAll( ".svg-chart-bar[data-svg-chart-draggable='true']" ).Count );
            Assert.Equal( 3, component.FindAll( ".svg-chart-data-drag-hit-target" ).Count );
        } );
    }

    [Fact]
    public void XMode_EnablesStackedHorizontalBarSegments()
    {
        var component = RenderBarChart( true );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( 6, component.FindAll( ".svg-chart-bar[data-svg-chart-draggable='true']" ).Count );
            Assert.Equal( 6, component.FindAll( ".svg-chart-data-drag-hit-target" ).Count );
        } );
    }

    [Fact]
    public void YMode_EnablesVerticalColumns()
    {
        List<DragSample> samples =
        [
            new() { Category = "A", Y = 10 },
            new() { Category = "B", Y = 20 },
            new() { Category = "C", Y = 30 }
        ];

        var component = Render<SvgColumnChart<DragSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgChartDataDrag>( 0 );
                builder.AddAttribute( 1, nameof( SvgChartDataDrag.Enabled ), true );
                builder.AddAttribute( 2, nameof( SvgChartDataDrag.Mode ), SvgChartDataDragMode.Y );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<DragSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartCategoryAxis<DragSample>.Value ), (Func<DragSample, object>)( item => item.Category ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgColumnSeries<DragSample>>( 5 );
                builder.AddAttribute( 6, nameof( SvgColumnSeries<DragSample>.Name ), "Samples" );
                builder.AddAttribute( 7, nameof( SvgColumnSeries<DragSample>.Value ), (Func<DragSample, double?>)( item => item.Y ) );
                builder.CloseComponent();
            } ) );

        component.WaitForAssertion( () => Assert.Equal( 3, component.FindAll( ".svg-chart-column[data-svg-chart-draggable='true']" ).Count ) );
    }

    [Fact]
    public void YMode_EnablesStackedAreaPoints()
    {
        List<DragSample> samples =
        [
            new() { Category = "A", Y = 10, Y2 = 5 },
            new() { Category = "B", Y = 20, Y2 = 10 },
            new() { Category = "C", Y = 30, Y2 = 15 }
        ];

        var component = Render<SvgAreaChart<DragSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgChartDataDrag>( 0 );
                builder.AddAttribute( 1, nameof( SvgChartDataDrag.Enabled ), true );
                builder.AddAttribute( 2, nameof( SvgChartDataDrag.Mode ), SvgChartDataDragMode.Y );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<DragSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartCategoryAxis<DragSample>.Value ), (Func<DragSample, object>)( item => item.Category ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartValueAxis>( 5 );
                builder.AddAttribute( 6, nameof( SvgChartValueAxis.Stacked ), true );
                builder.CloseComponent();

                builder.OpenComponent<SvgAreaSeries<DragSample>>( 7 );
                builder.AddAttribute( 8, nameof( SvgAreaSeries<DragSample>.Name ), "Primary" );
                builder.AddAttribute( 9, nameof( SvgAreaSeries<DragSample>.Stack ), "values" );
                builder.AddAttribute( 10, nameof( SvgAreaSeries<DragSample>.Value ), (Func<DragSample, double?>)( item => item.Y ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgAreaSeries<DragSample>>( 11 );
                builder.AddAttribute( 12, nameof( SvgAreaSeries<DragSample>.Name ), "Secondary" );
                builder.AddAttribute( 13, nameof( SvgAreaSeries<DragSample>.Stack ), "values" );
                builder.AddAttribute( 14, nameof( SvgAreaSeries<DragSample>.Value ), (Func<DragSample, double?>)( item => item.Y2 ) );
                builder.CloseComponent();
            } ) );

        component.WaitForAssertion( () => Assert.Equal( 6, component.FindAll( ".svg-chart-area-marker[data-svg-chart-draggable='true']" ).Count ) );
    }

    [Fact]
    public void YMode_EnablesRadialChartPoints()
    {
        var pie = RenderRadialChart<SvgPieChart<object>>();
        var doughnut = RenderRadialChart<SvgDoughnutChart<object>>();
        var polarArea = RenderRadialChart<SvgPolarAreaChart<object>>();
        var radar = RenderRadialChart<SvgRadarChart<object>>();

        pie.WaitForAssertion( () => Assert.Equal( 3, pie.FindAll( ".svg-chart-pie-segment[data-svg-chart-draggable='true']" ).Count ) );
        doughnut.WaitForAssertion( () => Assert.Equal( 3, doughnut.FindAll( ".svg-chart-doughnut-segment[data-svg-chart-draggable='true']" ).Count ) );
        polarArea.WaitForAssertion( () => Assert.Equal( 3, polarArea.FindAll( ".svg-chart-polararea-segment[data-svg-chart-draggable='true']" ).Count ) );
        radar.WaitForAssertion( () => Assert.Equal( 3, radar.FindAll( ".svg-chart-radar-marker[data-svg-chart-draggable='true']" ).Count ) );
    }

    private IRenderedComponent<SvgLineChart<DragSample>> RenderLineChart( Func<SvgChartPointEventArgs, bool> canDrag = null, bool draggable = true, SvgChartOptions options = null )
    {
        List<DragSample> samples =
        [
            new() { Category = "A", Y = 10 },
            new() { Category = "B", Y = 20 },
            new() { Category = "C", Y = 30 }
        ];

        return Render<SvgLineChart<DragSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .Add( chart => chart.Options, options )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgChartDataDrag>( 0 );
                builder.AddAttribute( 1, nameof( SvgChartDataDrag.Enabled ), true );
                builder.AddAttribute( 2, nameof( SvgChartDataDrag.HitRadius ), 18d );
                builder.AddAttribute( 3, nameof( SvgChartDataDrag.CanDrag ), canDrag );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<DragSample>>( 4 );
                builder.AddAttribute( 5, nameof( SvgChartCategoryAxis<DragSample>.Value ), (Func<DragSample, object>)( item => item.Category ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgLineSeries<DragSample>>( 6 );
                builder.AddAttribute( 7, nameof( SvgLineSeries<DragSample>.Name ), "Samples" );
                builder.AddAttribute( 8, nameof( SvgLineSeries<DragSample>.Value ), (Func<DragSample, double?>)( item => item.Y ) );
                builder.AddAttribute( 9, nameof( SvgLineSeries<DragSample>.Draggable ), draggable );
                builder.CloseComponent();
            } ) );
    }

    private IRenderedComponent<SvgBarChart<DragSample>> RenderBarChart( bool stacked = false )
    {
        List<DragSample> samples =
        [
            new() { Category = "A", Y = 10, Y2 = 5 },
            new() { Category = "B", Y = 20, Y2 = 10 },
            new() { Category = "C", Y = 30, Y2 = 15 }
        ];

        return Render<SvgBarChart<DragSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgChartDataDrag>( 0 );
                builder.AddAttribute( 1, nameof( SvgChartDataDrag.Enabled ), true );
                builder.AddAttribute( 2, nameof( SvgChartDataDrag.Mode ), SvgChartDataDragMode.X );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartCategoryAxis<DragSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartCategoryAxis<DragSample>.Value ), (Func<DragSample, object>)( item => item.Category ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartValueAxis>( 5 );
                builder.AddAttribute( 6, nameof( SvgChartValueAxis.Stacked ), stacked );
                builder.CloseComponent();

                builder.OpenComponent<SvgBarSeries<DragSample>>( 7 );
                builder.AddAttribute( 8, nameof( SvgBarSeries<DragSample>.Name ), "Primary" );
                builder.AddAttribute( 9, nameof( SvgBarSeries<DragSample>.Stack ), stacked ? "values" : null );
                builder.AddAttribute( 10, nameof( SvgBarSeries<DragSample>.Value ), (Func<DragSample, double?>)( item => item.Y ) );
                builder.CloseComponent();

                if ( stacked )
                {
                    builder.OpenComponent<SvgBarSeries<DragSample>>( 11 );
                    builder.AddAttribute( 12, nameof( SvgBarSeries<DragSample>.Name ), "Secondary" );
                    builder.AddAttribute( 13, nameof( SvgBarSeries<DragSample>.Stack ), "values" );
                    builder.AddAttribute( 14, nameof( SvgBarSeries<DragSample>.Value ), (Func<DragSample, double?>)( item => item.Y2 ) );
                    builder.CloseComponent();
                }
            } ) );
    }

    private IRenderedComponent<TChart> RenderRadialChart<TChart>()
        where TChart : SvgChart<object>
    {
        var data = new SvgChartData<double?>
        {
            Labels = ["A", "B", "C"],
            Series =
            [
                new()
                {
                    Name = "Samples",
                    Values = [20, 40, 60]
                }
            ]
        };
        var options = new SvgChartOptions
        {
            DataDrag = new()
            {
                Enabled = true,
                Mode = SvgChartDataDragMode.Y
            },
            YAxis = new()
            {
                Min = 0,
                Max = 100
            }
        };

        return Render<TChart>( parameters => parameters
            .Add( chart => chart.Data, data )
            .Add( chart => chart.Options, options ) );
    }

    private sealed class DragSample
    {
        public string Category { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Y2 { get; set; }
    }
}