#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public partial class LineChart<TItem> : BaseChart<LineChartDataset<TItem>, TItem, LineChartOptions, LineChartModel>
    {
        #region Members

        #endregion

        #region Constructors

        public LineChart()
        {
            Type = ChartType.Line;
        }

        #endregion

        #region Properties

        #endregion
    }
}
