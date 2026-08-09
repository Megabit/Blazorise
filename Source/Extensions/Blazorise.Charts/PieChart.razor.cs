#region Using directives
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a pie chart.
/// </summary>
public partial class PieChart<TItem> : BaseChart<PieChartDataset<TItem>, TItem, PieChartOptions, PieChartModel>
{
    #region Members

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a pie chart instance.
    /// </summary>
    public PieChart()
    {
        Type = ChartType.Pie;
    }

    #endregion

    #region Properties

    #endregion
}