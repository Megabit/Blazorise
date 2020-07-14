#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public partial class HorizontalBarChart<TItem> : BaseChart<BarChartDataset<TItem>, TItem, BarChartOptions, BarChartModel>
    {
        #region Members

        #endregion

        #region Constructors

        public HorizontalBarChart()
        {
            Type = ChartType.HorizontalBar;
        }

        #endregion

        #region Properties

        #endregion
    }
}
