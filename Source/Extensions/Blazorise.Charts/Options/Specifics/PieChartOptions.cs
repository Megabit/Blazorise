#region Using directives
using System;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class PieChartOptions : ChartOptions
    {
        /// <summary>
        /// The percentage of the chart that is cut out of the middle.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? CutoutPercentage { get; set; } = 0;

        /// <summary>
        /// Starting angle to draw arcs from.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Rotation { get; set; } = -0.5 * Math.PI;

        /// <summary>
        /// Sweep to allow arcs to cover.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Circumference { get; set; } = 2 * Math.PI;
    }
}