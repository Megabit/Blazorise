#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Point elements are used to represent the points in a line, radar or bubble chart.
/// </summary>
public class ChartElements
{
    /// <summary>
    /// Point elements are used to represent the points in a line, radar or bubble chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartPointElements Point { get; set; }

    /// <summary>
    /// Line elements are used to represent the line in a line chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartLineElements Line { get; set; }

    /// <summary>
    /// Bar elements are used to represent the bars in a bar chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartBarElements Bar { get; set; }

    /// <summary>
    /// Arcs are used in the polar area, doughnut and pie charts.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartArcElements Arc { get; set; }
}