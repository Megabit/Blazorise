using System;
using System.Collections.Generic;
using Blazorise.Charts.Svg;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

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

    private IRenderedComponent<SvgScatterChart<TimeSample>> RenderTimeChart( List<TimeSample> samples, TimeZoneInfo timeZone )
    {
        return Render<SvgScatterChart<TimeSample>>( parameters => parameters
            .Add( chart => chart.Items, samples )
            .AddChildContent( builder =>
            {
                builder.OpenComponent<SvgScatterSeries<TimeSample>>( 0 );
                builder.AddAttribute( 1, nameof( SvgScatterSeries<TimeSample>.Name ), "Samples" );
                builder.AddAttribute( 2, nameof( SvgScatterSeries<TimeSample>.YValue ), (Func<TimeSample, double?>)( item => item.Value ) );
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