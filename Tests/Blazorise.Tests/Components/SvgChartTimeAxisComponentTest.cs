#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Charts.Svg;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class SvgChartTimeAxisComponentTest : BunitContext
{
    public SvgChartTimeAxisComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void ContinuousTimeAxis_InterpretsUnspecifiedValuesInConfiguredTimeZone()
    {
        TimeZoneInfo timeZone = CreateTestTimeZone();
        List<TimeSample> samples =
        [
            new() { Time = new DateTime( 2026, 7, 15, 7, 0, 0, DateTimeKind.Unspecified ), Value = 1 },
            new() { Time = new DateTime( 2026, 7, 15, 8, 0, 0, DateTimeKind.Unspecified ), Value = 2 }
        ];

        IRenderedComponent<SvgScatterChart<TimeSample>> component = RenderTimeChart( samples, timeZone );

        component.WaitForAssertion( () => Assert.Contains( "07:00", component.Markup ) );

        component.Find( ".svg-chart-point.svg-chart-scatter" ).MouseEnter();

        component.WaitForAssertion( () => Assert.Contains( "07:00, 1. Samples.", component.Markup ) );
    }

    [Fact]
    public void ContinuousTimeAxis_ConvertsUtcValuesToConfiguredTimeZone()
    {
        TimeZoneInfo timeZone = CreateTestTimeZone();
        List<TimeSample> samples =
        [
            new() { Time = new DateTime( 2026, 7, 15, 5, 0, 0, DateTimeKind.Utc ), Value = 1 },
            new() { Time = new DateTime( 2026, 7, 15, 6, 0, 0, DateTimeKind.Utc ), Value = 2 }
        ];

        IRenderedComponent<SvgScatterChart<TimeSample>> component = RenderTimeChart( samples, timeZone );

        component.WaitForAssertion( () => Assert.Contains( "07:00", component.Markup ) );

        component.Find( ".svg-chart-point.svg-chart-scatter" ).MouseEnter();

        component.WaitForAssertion( () => Assert.Contains( "07:00, 1. Samples.", component.Markup ) );
    }

    [Fact]
    public void ContinuousTimeAxis_UsesOnlyRenderedPointsForAutomaticRange()
    {
        TimeZoneInfo timeZone = CreateTestTimeZone();
        List<TimeSample> samples =
        [
            new() { Time = new DateTime( 2026, 7, 15, 7, 26, 0, DateTimeKind.Unspecified ), Value = null },
            new() { Time = new DateTime( 2026, 7, 15, 8, 25, 0, DateTimeKind.Unspecified ), Value = 170 },
            new() { Time = new DateTime( 2026, 7, 15, 10, 0, 0, DateTimeKind.Unspecified ), Value = 175 },
            new() { Time = new DateTime( 2026, 7, 15, 15, 46, 0, DateTimeKind.Unspecified ), Value = null }
        ];

        IRenderedComponent<SvgLineChart<TimeSample>> component = RenderLineTimeChart( samples, timeZone );

        component.WaitForAssertion( () =>
        {
            Assert.Contains( "08:25", component.Markup );
            Assert.DoesNotContain( "07:26", component.Markup );
            Assert.DoesNotContain( "15:46", component.Markup );
        } );
    }

    [Theory]
    [InlineData( 7, 40, 13, 13, "08:00,10:00,12:00" )]
    [InlineData( 7, 0, 8, 0, "07:00,08:00" )]
    [InlineData( 7, 40, 7, 45, "07:40,07:45" )]
    public void ContinuousTimeAxis_UsesExactBoundsAndRegularTicks( int startHour, int startMinute, int endHour, int endMinute, string expectedLabels )
    {
        var timeZone = CreateTestTimeZone();
        var samples = new List<TimeSample>
        {
            new() { Time = new DateTime( 2026, 7, 15, startHour, startMinute, 0 ), Value = 1 },
            new() { Time = new DateTime( 2026, 7, 15, endHour, endMinute, 0 ), Value = 2 },
        };

        var component = RenderTimeChart( samples, timeZone, SvgChartTimeUnit.Hour );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( expectedLabels.Split( ',' ), component.FindAll( ".svg-chart-point-xaxis-labels text" ).Select( label => label.TextContent ) );

            var plot = component.Find( "clipPath rect" );
            var points = component.FindAll( ".svg-chart-point.svg-chart-scatter" );
            var plotStart = double.Parse( plot.GetAttribute( "x" ), CultureInfo.InvariantCulture );
            var plotWidth = double.Parse( plot.GetAttribute( "width" ), CultureInfo.InvariantCulture );

            Assert.Equal( plotStart - 0.5, double.Parse( points[0].GetAttribute( "cx" ), CultureInfo.InvariantCulture ), 3 );
            Assert.Equal( plotStart + plotWidth, double.Parse( points[1].GetAttribute( "cx" ), CultureInfo.InvariantCulture ), 3 );
        } );
    }

    [Fact]
    public void ContinuousTimeAxis_PreservesExplicitBounds()
    {
        var timeZone = CreateTestTimeZone();
        var samples = new List<TimeSample>
        {
            new() { Time = new DateTime( 2026, 7, 15, 7, 40, 0 ), Value = 1 },
            new() { Time = new DateTime( 2026, 7, 15, 13, 13, 0 ), Value = 2 },
        };

        var options = new SvgChartOptions
        {
            XAxis = new()
            {
                Min = new DateTimeOffset( 2026, 7, 15, 6, 0, 0, timeZone.BaseUtcOffset ).ToUnixTimeMilliseconds(),
                Max = new DateTimeOffset( 2026, 7, 15, 14, 0, 0, timeZone.BaseUtcOffset ).ToUnixTimeMilliseconds(),
            },
        };

        var component = RenderTimeChart( samples, timeZone, SvgChartTimeUnit.Hour, options );

        component.WaitForAssertion( () => Assert.Equal(
            new[] { "06:00", "08:00", "10:00", "12:00", "14:00" },
            component.FindAll( ".svg-chart-point-xaxis-labels text" ).Select( label => label.TextContent ) ) );
    }

    [Fact]
    public void UnlicensedChart_KeepsPointLimitBeforeAndAfterInteraction()
    {
        var samples = Enumerable.Range( 0, 16 )
            .Select( index => new TimeSample
            {
                Time = new DateTime( 2026, 8, 13, 10, 15, 0 ).AddMinutes( index * 33 ),
                Value = index + 1,
            } )
            .ToList();

        var component = RenderTimeChart( samples, CreateTestTimeZone() );

        Assert.Equal( 10, component.FindAll( ".svg-chart-point.svg-chart-scatter" ).Count );

        component.Find( ".svg-chart-point.svg-chart-scatter" ).MouseEnter();

        Assert.Equal( 10, component.FindAll( ".svg-chart-point.svg-chart-scatter" ).Count );

        component.Find( ".svg-chart-legend-item" ).Click();
        component.Find( ".svg-chart-legend-item" ).Click();

        Assert.Equal( 10, component.FindAll( ".svg-chart-point.svg-chart-scatter" ).Count );
    }

    [Fact]
    public void AxisLabels_UseAxisFontOptions()
    {
        TimeZoneInfo timeZone = CreateTestTimeZone();
        List<TimeSample> samples =
        [
            new() { Time = new DateTime( 2026, 7, 15, 7, 0, 0, DateTimeKind.Unspecified ), Value = 1 },
            new() { Time = new DateTime( 2026, 7, 15, 8, 0, 0, DateTimeKind.Unspecified ), Value = 2 }
        ];

        IRenderedComponent<SvgScatterChart<TimeSample>> component = Render<SvgScatterChart<TimeSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgScatterSeries<TimeSample>>( 0 );
                builder.AddAttribute( 1, nameof( SvgScatterSeries<TimeSample>.YValue ), (Func<TimeSample, double?>)( item => item.Value ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartTimeAxis<TimeSample>>( 2 );
                builder.AddAttribute( 3, nameof( SvgChartTimeAxis<TimeSample>.TimeValue ), (Func<TimeSample, DateTime?>)( item => item.Time ) );
                builder.AddAttribute( 4, nameof( SvgChartTimeAxis<TimeSample>.Scale ), SvgChartTimeScale.Continuous );
                builder.AddAttribute( 5, nameof( SvgChartTimeAxis<TimeSample>.LabelsOptions ), new SvgChartAxisLabelsOptions
                {
                    Font = new() { Size = 9 }
                } );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartValueAxis>( 6 );
                builder.AddAttribute( 7, nameof( SvgChartValueAxis.LabelsOptions ), new SvgChartAxisLabelsOptions
                {
                    Font = new() { Size = 13 }
                } );
                builder.CloseComponent();
            } ) );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( "9", component.Find( ".svg-chart-point-xaxis-labels text" ).GetAttribute( "font-size" ) );
            Assert.Equal( "13", component.Find( ".svg-chart-grid > text" ).GetAttribute( "font-size" ) );
        } );
    }

    private IRenderedComponent<SvgScatterChart<TimeSample>> RenderTimeChart( List<TimeSample> samples, TimeZoneInfo timeZone, SvgChartTimeUnit unit = SvgChartTimeUnit.Minute, SvgChartOptions options = null )
    {
        return Render<SvgScatterChart<TimeSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .Add( chart => chart.Options, options )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgScatterSeries<TimeSample>>( 0 );
                builder.AddAttribute( 1, nameof( SvgScatterSeries<TimeSample>.Name ), "Samples" );
                builder.AddAttribute( 2, nameof( SvgScatterSeries<TimeSample>.YValue ), (Func<TimeSample, double?>)( item => item.Value ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartTimeAxis<TimeSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartTimeAxis<TimeSample>.TimeValue ), (Func<TimeSample, DateTime?>)( item => item.Time ) );
                builder.AddAttribute( 5, nameof( SvgChartTimeAxis<TimeSample>.Scale ), SvgChartTimeScale.Continuous );
                builder.AddAttribute( 6, nameof( SvgChartTimeAxis<TimeSample>.Unit ), unit );
                builder.AddAttribute( 7, nameof( SvgChartTimeAxis<TimeSample>.Format ), "HH:mm" );
                builder.AddAttribute( 8, nameof( SvgChartTimeAxis<TimeSample>.TimeZone ), timeZone );
                builder.CloseComponent();
            } ) );
    }

    private IRenderedComponent<SvgLineChart<TimeSample>> RenderLineTimeChart( List<TimeSample> samples, TimeZoneInfo timeZone )
    {
        return Render<SvgLineChart<TimeSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgLineSeries<TimeSample>>( 0 );
                builder.AddAttribute( 1, nameof( SvgLineSeries<TimeSample>.Name ), "Samples" );
                builder.AddAttribute( 2, nameof( SvgLineSeries<TimeSample>.Value ), (Func<TimeSample, double?>)( item => item.Value ) );
                builder.CloseComponent();

                builder.OpenComponent<SvgChartTimeAxis<TimeSample>>( 3 );
                builder.AddAttribute( 4, nameof( SvgChartTimeAxis<TimeSample>.TimeValue ), (Func<TimeSample, DateTime?>)( item => item.Time ) );
                builder.AddAttribute( 5, nameof( SvgChartTimeAxis<TimeSample>.Scale ), SvgChartTimeScale.Continuous );
                builder.AddAttribute( 6, nameof( SvgChartTimeAxis<TimeSample>.Unit ), SvgChartTimeUnit.Minute );
                builder.AddAttribute( 7, nameof( SvgChartTimeAxis<TimeSample>.Format ), "HH:mm" );
                builder.AddAttribute( 8, nameof( SvgChartTimeAxis<TimeSample>.TimeZone ), timeZone );
                builder.CloseComponent();
            } ) );
    }

    private static TimeZoneInfo CreateTestTimeZone()
    {
        return TimeZoneInfo.CreateCustomTimeZone( "SvgChartTimeAxisTest", TimeSpan.FromHours( 2 ), "UTC+02:00", "UTC+02:00" );
    }

    private sealed class TimeSample
    {
        public DateTime Time { get; set; }

        public double? Value { get; set; }
    }
}