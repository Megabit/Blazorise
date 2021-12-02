#region Using directives
using System;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class PolarAreaChartOptions : ChartOptions
    {
        /// <summary>
        /// Starting angle to draw arcs for the first item in a dataset.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? StartAngle { get; set; } = -0.5 * Math.PI;
    }
}