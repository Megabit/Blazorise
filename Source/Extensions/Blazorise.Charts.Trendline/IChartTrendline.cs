#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts.Trendline;

/// <summary>
/// Interface for the trendline plugin.
/// </summary>
public interface IChartTrendline
{
    /// <summary>
    /// Asynchronously adds options for trend lines to a chart.
    /// </summary>
    /// <param name="trendlineData">Contains the data needed to configure the trend lines for the chart.</param>
    /// <returns>This method does not return a value.</returns>
    Task AddTrendLineOptions( List<ChartTrendlineData> trendlineData );
}
