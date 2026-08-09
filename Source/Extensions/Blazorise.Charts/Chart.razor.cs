#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Renders a Chart.js visualization.
/// </summary>
public partial class Chart<TItem> : BaseChart<ChartDataset<TItem>, TItem, ChartOptions, ChartModel>
{
}