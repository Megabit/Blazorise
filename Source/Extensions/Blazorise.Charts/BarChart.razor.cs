#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public partial class BarChart<TItem> : BaseChart<BarChartDataset<TItem>, TItem, BarChartOptions, BarChartModel>
    {
        #region Members

        #endregion

        #region Constructors

        public BarChart()
        {
            Type = ChartType.Bar;
        }

        #endregion

        #region Properties

        #endregion
    }
}
