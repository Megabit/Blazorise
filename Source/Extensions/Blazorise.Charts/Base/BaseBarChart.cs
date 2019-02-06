#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Base
{
    public abstract class BaseBarChart<TItem> : BaseChart<BarChartDataset<TItem>, TItem, BarChartOptions>
    {
        #region Members

        #endregion

        #region Methods

        #endregion

        #region Properties

        protected override ChartType Type { get; set; } = ChartType.Bar;

        #endregion
    }
}
