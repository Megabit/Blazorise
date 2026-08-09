#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Stores values and styling for doughnut chart data.
/// </summary>
public class DoughnutChartDataset<T> : PieChartDataset<T>
{
    /// <summary>
    /// Creates a doughnut chart dataset.
    /// </summary>
    public DoughnutChartDataset()
        : base()
    {
        Type = "doughnut";
    }
}