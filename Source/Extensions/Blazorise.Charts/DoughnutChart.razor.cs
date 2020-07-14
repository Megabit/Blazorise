﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
    public partial class DoughnutChart<TItem> : BaseChart<DoughnutChartDataset<TItem>, TItem, DoughnutChartOptions, DoughnutChartModel>
    {
        #region Members

        #endregion

        #region Constructors

        public DoughnutChart()
        {
            Type = ChartType.Doughnut;
        }

        #endregion

        #region Properties

        #endregion
    }
}
