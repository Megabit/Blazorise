#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a line chart.
/// </summary>
public partial class LineChart<TItem> : BaseChart<LineChartDataset<TItem>, TItem, LineChartOptions, LineChartModel>
{
    #region Members

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a line chart instance.
    /// </summary>
    public LineChart()
    {
        Type = ChartType.Line;
    }

    #endregion

    #region Properties

    #endregion
}