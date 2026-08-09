#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a bar chart.
/// </summary>
/// <typeparam name="TItem">Value type stored by the chart datasets.</typeparam>
public partial class BarChart<TItem> : BaseChart<BarChartDataset<TItem>, TItem, BarChartOptions, BarChartModel>
{
    #region Members

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a bar chart configured for bar datasets.
    /// </summary>
    public BarChart()
    {
        Type = ChartType.Bar;
    }

    #endregion

    #region Properties

    #endregion
}