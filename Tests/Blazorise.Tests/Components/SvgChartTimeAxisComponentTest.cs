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

    private static TimeZoneInfo CreateTestTimeZone()
    {
        return TimeZoneInfo.CreateCustomTimeZone( "SvgChartTimeAxisTest", TimeSpan.FromHours( 2 ), "UTC+02:00", "UTC+02:00" );
    }

    private sealed class TimeSample
    {
        public DateTime Time { get; set; }

        public double Value { get; set; }
    }
}