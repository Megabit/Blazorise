using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Charts.Svg;

namespace Blazorise.Demo.Pages.Tests;

public partial class ChartsSvgStreamingPage : IAsyncDisposable
{
    private readonly Random random = new( DateTime.Now.Millisecond );

    private SvgLineChart<object> latencyChart;

    private SvgAreaChart<object> throughputChart;

    private SvgColumnChart<object> queueChart;

    private CancellationTokenSource streamingCancellationTokenSource;

    private Task streamingTask;

    private bool streamingPaused;

    private string lastEvent = "Interact with a streamed point to see event details.";

    private SvgChartData<double?> latencyData = CreateLatencyData();

    private SvgChartData<double?> throughputData = CreateThroughputData();

    private SvgChartData<double?> queueData = CreateQueueData();

    private SvgChartOptions latencyOptions = new()
    {
        Height = 380,
        Legend = new() { Visible = false },
        XAxis = new()
        {
            GridLines = new() { Visible = true },
            Labels = new()
            {
                Step = 4,
                Offset = 30,
            },
        },
    };

    private SvgChartOptions throughputOptions = new()
    {
        Height = 380,
        Legend = new() { Position = SvgChartLegendPosition.Bottom },
        XAxis = new()
        {
            GridLines = new() { Visible = true },
            Labels = new()
            {
                Step = 3,
                Offset = 30,
            },
        },
    };

    private SvgChartOptions queueOptions = new()
    {
        Height = 380,
        Legend = new() { Visible = false },
        XAxis = new()
        {
            Labels = new()
            {
                Step = 2,
                Offset = 30,
            },
        },
    };

    private SvgChartStreamingOptions infiniteStreamingOptions = new()
    {
        Enabled = true,
        MaxDataPoints = null,
        VisibleDataPoints = 20,
        Duration = null,
        Reverse = false,
        Animation = new()
        {
            Duration = TimeSpan.FromSeconds( 1 ),
        },
        RefreshInterval = TimeSpan.FromMilliseconds( 500 ),
    };

    private SvgChartStreamingOptions rollingStreamingOptions = new()
    {
        Enabled = true,
        MaxDataPoints = 80,
        VisibleDataPoints = 18,
        Duration = TimeSpan.FromSeconds( 45 ),
        Reverse = false,
        Animation = new()
        {
            Duration = TimeSpan.FromMilliseconds( 900 ),
        },
        RefreshInterval = TimeSpan.FromMilliseconds( 500 ),
    };

    private SvgChartStreamingOptions reverseStreamingOptions = new()
    {
        Enabled = true,
        MaxDataPoints = 60,
        VisibleDataPoints = 14,
        Duration = null,
        Reverse = true,
        Animation = new()
        {
            Duration = TimeSpan.FromMilliseconds( 850 ),
        },
        RefreshInterval = TimeSpan.FromMilliseconds( 500 ),
    };

    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            streamingCancellationTokenSource = new();
            streamingTask = RunStreamingAsync( streamingCancellationTokenSource.Token );
        }

        return Task.CompletedTask;
    }

    private async Task RunStreamingAsync( CancellationToken cancellationToken )
    {
        try
        {
            using PeriodicTimer timer = new( TimeSpan.FromSeconds( 1 ) );

            while ( await timer.WaitForNextTickAsync( cancellationToken ) )
            {
                await InvokeAsync( AppendStreamingValues );
            }
        }
        catch ( OperationCanceledException )
        {
        }
    }

    private async Task AppendStreamingValues()
    {
        string label = DateTime.Now.ToString( "HH:mm:ss" );

        if ( latencyChart is not null )
            await latencyChart.AppendValue( "Latency", label, NextValue( 20, 95 ) );

        if ( throughputChart is not null )
        {
            await throughputChart.AppendValues( label, new Dictionary<string, double?>
            {
                ["Reads"] = NextValue( 60, 140 ),
                ["Writes"] = NextValue( 30, 95 ),
            } );
        }

        if ( queueChart is not null )
            await queueChart.AppendValue( "Queue", label, NextValue( 4, 32 ) );
    }

    private async Task ToggleStreaming()
    {
        if ( streamingPaused )
        {
            if ( latencyChart is not null )
                await latencyChart.ResumeStreaming();

            if ( throughputChart is not null )
                await throughputChart.ResumeStreaming();

            if ( queueChart is not null )
                await queueChart.ResumeStreaming();

            streamingPaused = false;
        }
        else
        {
            if ( latencyChart is not null )
                await latencyChart.PauseStreaming();

            if ( throughputChart is not null )
                await throughputChart.PauseStreaming();

            if ( queueChart is not null )
                await queueChart.PauseStreaming();

            streamingPaused = true;
        }
    }

    private async Task ClearStreaming()
    {
        latencyData = CreateLatencyData();
        throughputData = CreateThroughputData();
        queueData = CreateQueueData();

        if ( latencyChart is not null )
            await latencyChart.SetData( latencyData );

        if ( throughputChart is not null )
            await throughputChart.SetData( throughputData );

        if ( queueChart is not null )
            await queueChart.SetData( queueData );
    }

    private Task OnPointClicked( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Clicked {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private Task OnPointHovered( SvgChartPointEventArgs eventArgs )
    {
        lastEvent = $"Hovered {eventArgs.SeriesName} / {eventArgs.Category}: {eventArgs.Value}";

        return Task.CompletedTask;
    }

    private double NextValue( int min, int max )
    {
        return Math.Round( min + random.NextDouble() * ( max - min ), 1 );
    }

    private static SvgChartData<double?> CreateLatencyData()
    {
        return new()
        {
            Series =
            [
                new()
                {
                    Name = "Latency",
                    Color = Color.Warning,
                },
            ]
        };
    }

    private static SvgChartData<double?> CreateThroughputData()
    {
        return new()
        {
            Series =
            [
                new()
                {
                    Name = "Reads",
                    Color = Color.Primary,
                },
                new()
                {
                    Name = "Writes",
                    Color = Color.Success,
                },
            ]
        };
    }

    private static SvgChartData<double?> CreateQueueData()
    {
        return new()
        {
            Series =
            [
                new()
                {
                    Name = "Queue",
                    Color = Color.Info,
                },
            ]
        };
    }

    public async ValueTask DisposeAsync()
    {
        streamingCancellationTokenSource?.Cancel();

        if ( streamingTask is not null )
            await streamingTask;

        streamingCancellationTokenSource?.Dispose();
    }
}