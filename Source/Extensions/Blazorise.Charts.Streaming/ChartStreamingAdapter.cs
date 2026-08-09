#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming;

/// <summary>
/// Routes browser refresh ticks back to a streaming chart component.
/// </summary>
public class ChartStreamingAdapter
{
    private readonly IChartStreaming chartStreaming;

    /// <summary>
    /// Connects the callback adapter to its chart.
    /// </summary>
    /// <param name="chartStreaming">Chart that supplies the next batch of data.</param>
    public ChartStreamingAdapter( IChartStreaming chartStreaming )
    {
        this.chartStreaming = chartStreaming;
    }

    /// <summary>
    /// Requests fresh data when the browser refresh interval elapses.
    /// </summary>
    [JSInvokable]
    public Task Refresh()
    {
        return chartStreaming.Refresh();
    }
}