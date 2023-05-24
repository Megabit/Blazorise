#region Using directives
using System;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

public class PieChartOptions : ChartOptions
{
    /// <summary>
    /// The percentage of the chart that is cut out of the middle.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? CutoutPercentage { get; set; }

    /// <summary>
    /// Starting angle to draw arcs from.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Sweep to allow arcs to cover.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Circumference { get; set; }
}