#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public abstract class BaseDoughnutChart<TItem> : BaseChart<DoughnutChartDataset<TItem>, TItem, DoughnutChartOptions>
    {
        #region Members

        #endregion

        #region Methods

        public BaseDoughnutChart()
        {
            Type = ChartType.Doughnut;
        }

        #endregion

        #region Properties

        #endregion
    }
}
