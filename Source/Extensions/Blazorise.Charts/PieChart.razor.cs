#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public abstract class BasePieChart<TItem> : BaseChart<PieChartDataset<TItem>, TItem, PieChartOptions>
    {
        #region Members

        #endregion

        #region Methods

        public BasePieChart()
        {
            Type = ChartType.Pie;
        }

        #endregion

        #region Properties

        #endregion
    }
}
