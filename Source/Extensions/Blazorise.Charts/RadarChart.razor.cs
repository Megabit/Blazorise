#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public abstract class BaseRadarChart<TItem> : BaseChart<RadarChartDataset<TItem>, TItem, RadarChartOptions, RadarChartModel>
    {
        #region Members

        #endregion

        #region Methods

        public BaseRadarChart()
        {
            Type = ChartType.Radar;
        }

        #endregion

        #region Properties

        #endregion
    }
}
