using System;
using System.Collections.Generic;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgAnimationPage
{
    private static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds( 550 );

    private readonly Random random = new( 12 );

    private List<AnimatedRevenue> Revenue = CreateRevenue();

    private List<TrafficSample> Traffic = CreateTraffic();

    private readonly SvgChartOptions columnOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
    };

    private readonly SvgChartOptions lineOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        Animation = new()
        {
            Enabled = true,
            Duration = TimeSpan.FromMilliseconds( 500 ),
            Easing = SvgChartAnimationEasing.EaseInOut,
            Geometry = new()
            {
                Enabled = true,
                Duration = TimeSpan.FromMilliseconds( 650 ),
                AnimatePosition = true,
                AnimateSize = true
            },
            Opacity = new()
            {
                Enabled = true,
                Duration = TimeSpan.FromMilliseconds( 850 ),
                Delay = TimeSpan.FromMilliseconds( 60 ),
                Easing = SvgChartAnimationEasing.EaseOut,
                From = 0.15
            },
            Stroke = new() { Enabled = false },
            Transform = new() { Enabled = false },
            Path = new() { Enabled = false },
        },
    };

    private void AddMonth()
    {
        var next = Revenue.Count + 1;

        Revenue.Add( new()
        {
            Month = $"M{next}",
            Revenue = 74 + random.Next( 0, 42 ),
            Forecast = 86 + random.Next( 0, 38 )
        } );
    }

    private void RandomizeRevenue()
    {
        foreach ( var item in Revenue )
        {
            item.Revenue = Math.Max( 30, item.Revenue + random.Next( -18, 19 ) );
            item.Forecast = Math.Max( 30, item.Forecast + random.Next( -16, 17 ) );
        }
    }

    private void ResetRevenue()
    {
        Revenue = CreateRevenue();
    }

    private void RandomizeTraffic()
    {
        foreach ( var item in Traffic )
        {
            item.Desktop = Math.Max( 20, item.Desktop + random.Next( -14, 15 ) );
            item.Mobile = Math.Max( 20, item.Mobile + random.Next( -16, 17 ) );
        }
    }

    private void ResetTraffic()
    {
        Traffic = CreateTraffic();
    }

    private static List<AnimatedRevenue> CreateRevenue()
    {
        return
        [
            new() { Month = "Jan", Revenue = 68, Forecast = 82 },
            new() { Month = "Feb", Revenue = 74, Forecast = 88 },
            new() { Month = "Mar", Revenue = 91, Forecast = 96 },
            new() { Month = "Apr", Revenue = 86, Forecast = 102 },
            new() { Month = "May", Revenue = 103, Forecast = 110 },
        ];
    }

    private static List<TrafficSample> CreateTraffic()
    {
        return
        [
            new() { Day = "Mon", Desktop = 48, Mobile = 42 },
            new() { Day = "Tue", Desktop = 61, Mobile = 58 },
            new() { Day = "Wed", Desktop = 73, Mobile = 66 },
            new() { Day = "Thu", Desktop = 68, Mobile = 70 },
            new() { Day = "Fri", Desktop = 82, Mobile = 78 },
            new() { Day = "Sat", Desktop = 76, Mobile = 84 },
        ];
    }

    private sealed class AnimatedRevenue
    {
        public string Month { get; set; }

        public double Revenue { get; set; }

        public double Forecast { get; set; }
    }

    private sealed class TrafficSample
    {
        public string Day { get; set; }

        public double Desktop { get; set; }

        public double Mobile { get; set; }
    }
}