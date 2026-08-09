#region Using directives
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a doughnut chart.
/// </summary>
public partial class DoughnutChart<TItem> : BaseChart<DoughnutChartDataset<TItem>, TItem, DoughnutChartOptions, DoughnutChartModel>
{
    #region Members

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a doughnut chart instance.
    /// </summary>
    public DoughnutChart()
    {
        Type = ChartType.Doughnut;
    }

    #endregion

    #region Properties

    #endregion
}