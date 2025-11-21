#region Using directives
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// The grid line configuration is nested under the scale configuration in the gridLines key. It defines options for the grid lines that run perpendicular to the axis.
/// </summary>
public class ChartAxisGridLine
{
    /// <summary>
    /// If true, gridlines are circular (on radar chart only).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Circular { get; set; }

    /// <summary>
    /// The color of the grid lines. If specified as an array, the first color applies to the first grid line, the second to the second grid line, and so on.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> Color { get; set; }

    /// <summary>
    /// If false, do not display grid lines for this axis.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// If true, draw lines on the chart area inside the axis lines. This is useful when there are multiple axes and you need to control which grid lines are drawn.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? DrawOnChartArea { get; set; }

    /// <summary>
    /// If true, draw lines beside the ticks in the axis area beside the chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? DrawTicks { get; set; }

    /// <summary>
    /// Stroke width of grid lines.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? LineWidth { get; set; }

    /// <summary>
    /// If true, grid lines will be shifted to be between labels. This is set to true for a bar chart by default.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Offset { get; set; }

    /// <summary>
    /// Length and spacing of the tick mark line. If not set, defaults to the grid line borderDash value.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<double> TickBorderDash { get; set; }

    /// <summary>
    /// Offset for the line dash of the tick mark. If unset, defaults to the grid line borderDashOffset value
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? TickBorderDashOffset { get; set; }

    /// <summary>
    /// Color of the tick line. If unset, defaults to the grid line color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> TickColor { get; set; }

    /// <summary>
    /// Length in pixels that the grid lines will draw into the axis area.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? TickLength { get; set; }

    /// <summary>
    /// Width of the tick mark in pixels. If unset, defaults to the grid line width.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? TickWidth { get; set; }

    /// <summary>
    /// z-index of gridline layer. Values &lt;= 0 are drawn under datasets, > 0 on top.
    /// </summary>
    [JsonPropertyName( "z" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Z { get; set; }
}