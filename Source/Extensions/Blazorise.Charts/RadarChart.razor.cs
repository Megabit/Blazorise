#region Using directives
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a radar chart.
/// </summary>
public partial class RadarChart<TItem> : BaseChart<RadarChartDataset<TItem>, TItem, RadarChartOptions, RadarChartModel>
{
    #region Members

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a radar chart instance.
    /// </summary>
    public RadarChart()
    {
        Type = ChartType.Radar;
    }

    #endregion

    #region Properties

    #endregion
}