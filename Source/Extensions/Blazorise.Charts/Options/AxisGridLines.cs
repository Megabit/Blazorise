#region Using directives
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// The grid line configuration is nested under the scale configuration in the gridLines key. It defines options for the grid lines that run perpendicular to the axis.
    /// </summary>
    [DataContract]
    public class AxisGridLines
    {
        /// <summary>
        /// If false, do not display grid lines for this axis.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Display { get; set; } = true;

        /// <summary>
        /// If true, gridlines are circular (on radar chart only).
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Circular { get; set; }

        /// <summary>
        /// The color of the grid lines.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Color { get; set; }

        /// <summary>
        /// Length and spacing of dashes on grid lines
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<int> BorderDash { get; set; }

        /// <summary>
        /// Offset for line dashes.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public decimal? BorderDashOffset { get; set; } = 0m;

        /// <summary>
        /// Stroke width of grid lines.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? LineWidth { get; set; } = 1;

        /// <summary>
        /// If true, draw border at the edge between the axis and the chart area.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? DrawBorder { get; set; } = true;

        /// <summary>
        /// If true, draw lines on the chart area inside the axis lines. This is useful when there are multiple axes and you need to control which grid lines are drawn.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? DrawOnChartArea { get; set; } = true;

        /// <summary>
        /// If true, draw lines beside the ticks in the axis area beside the chart.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? DrawTicks { get; set; } = true;

        /// <summary>
        /// Length in pixels that the grid lines will draw into the axis area.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? TickMarkLength { get; set; } = 10;

        /// <summary>
        /// Stroke width of the grid line for the first index (index 0).
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? ZeroLineWidth { get; set; } = 1;

        /// <summary>
        /// Stroke color of the grid line for the first index (index 0).
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string ZeroLineColor { get; set; }

        /// <summary>
        /// Length and spacing of dashes of the grid line for the first index (index 0).
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<int> ZeroLineBorderDash { get; set; }

        /// <summary>
        /// Offset for line dashes of the grid line for the first index (index 0).
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public decimal? ZeroLineBorderDashOffset { get; set; } = 0m;

        /// <summary>
        /// If true, grid lines will be shifted to be between labels. This is set to true for a category scale in a bar chart by default.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? OffsetGridLines { get; set; }
    }
}
