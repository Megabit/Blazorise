#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline
{
    public class ChartTrendlineAdapter
    {
        private readonly IChartTrendline chartTrendline;

        public ChartTrendlineAdapter( IChartTrendline chartTrendline )
        {
            this.chartTrendline = chartTrendline;
        }

        [JSInvokable]
        public Task Refresh()
        {
            return chartTrendline.Refresh();
        }
    }
}
