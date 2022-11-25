#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// The global options for the chart layout is defined in Chart.defaults.layout.
/// </summary>
public class ChartLayout
{
    /// <summary>
    /// Apply automatic padding so visible elements are completely drawn.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? AutoPadding { get; set; }

    /// <summary>
    /// The padding to add inside the chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartPadding Padding { get; set; }
}