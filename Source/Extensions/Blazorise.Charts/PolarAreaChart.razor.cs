#region Using directives
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a polar area chart.
/// </summary>
public partial class PolarAreaChart<TItem> : BaseChart<PolarAreaChartDataset<TItem>, TItem, PolarAreaChartOptions, PolarChartModel>
{
    #region Members

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a polar area chart instance.
    /// </summary>
    public PolarAreaChart()
    {
        Type = ChartType.PolarArea;
    }

    #endregion

    #region Properties

    #endregion
}