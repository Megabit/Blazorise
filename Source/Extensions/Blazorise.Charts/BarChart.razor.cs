#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public abstract class BaseBarChart<TItem> : BaseChart<BarChartDataset<TItem>, TItem, BarChartOptions>
    {
        #region Members

        #endregion

        #region Methods

        public BaseBarChart()
        {
            Type = ChartType.Bar;
        }

        #endregion

        #region Properties

        #endregion
    }
}
