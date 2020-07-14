#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming
{
    public class ChartStreamingAdapter
    {
        private readonly IChartStreaming chartStreaming;

        public ChartStreamingAdapter( IChartStreaming chartStreaming )
        {
            this.chartStreaming = chartStreaming;
        }

        [JSInvokable]
        public Task Refresh()
        {
            return chartStreaming.Refresh();
        }
    }
}
