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
        var samples = new List<DragSample>
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

    private IRenderedComponent<SvgLineChart<DragSample>> RenderLineChart( Func<SvgChartPointEventArgs, bool> canDrag = null, bool draggable = true, SvgChartOptions options = null )
    {
        var samples = new List<DragSample>
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

    private sealed class DragSample
    {
        public string Category { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}