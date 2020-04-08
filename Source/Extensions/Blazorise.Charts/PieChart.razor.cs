#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
