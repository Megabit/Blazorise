#region Using directives
using System;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

public class PieChartOptions : ChartOptions
{
    /// <summary>
    /// The portion of the chart that is cut out of the middle. If string and ending with '%', percentage of the chart radius. number is considered to be pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Cutout { get; set; }

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