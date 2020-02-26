#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public partial class PolarAreaChart<TItem> : BaseChart<PolarAreaChartDataset<TItem>, TItem, PolarAreaChartOptions, PolarChartModel>
    {
        #region Members

        #endregion

        #region Constructors

        public PolarAreaChart()
        {
            Type = ChartType.PolarArea;
        }

        #endregion

        #region Properties

        #endregion
    }
}
