#region Using directives
#endregion

namespace Blazorise.Charts
{
    public partial class PieChart<TItem> : BaseChart<PieChartDataset<TItem>, TItem, PieChartOptions, PieChartModel>
    {
        #region Members

        #endregion

        #region Constructors

        public PieChart()
        {
            Type = ChartType.Pie;
        }

        #endregion

        #region Properties

        #endregion
    }
}
