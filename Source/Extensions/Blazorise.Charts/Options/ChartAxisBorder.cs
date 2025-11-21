#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Chart.js v4 moved border styling for scales into a dedicated border object.
/// </summary>
public class ChartAxisBorder
{
    /// <summary>
    /// If true, draw border at the edge between the axis and the chart area.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// The color of the border line.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> Color { get; set; }

    /// <summary>
    /// The width of the border line.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Width { get; set; }

    /// <summary>
    /// Length and spacing of dashes on grid lines.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<int> Dash { get; set; }

    /// <summary>
    /// Offset for line dashes.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? DashOffset { get; set; }

    /// <summary>
    /// z-index of border layer. Values &lt;= 0 are drawn under datasets, &gt; 0 on top.
    /// </summary>
    [JsonPropertyName( "z" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Z { get; set; }
}