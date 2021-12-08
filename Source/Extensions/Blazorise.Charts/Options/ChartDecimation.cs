#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class ChartDecimation
    {
        /// <summary>
        /// s decimation enabled?
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Enabled { get; set; } = false;

        /// <summary>
        /// Decimation algorithm to use. See <see href="https://www.chartjs.org/docs/latest/configuration/decimation.html#decimation-algorithms">more...</see>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Algorithm { get; set; } = "min-max";

        /// <summary>
        /// If the <c>lttb</c> algorithm is used, this is the number of samples in the output dataset. Defaults to the canvas width to pick 1 sample per pixel.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Samples { get; set; }

        /// <summary>
        /// If the number of samples in the current axis range is above this value, the decimation will be triggered. Defaults to 4 times the canvas width. The number of point after decimation can be higher than the <c>Threshold</c> value.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Threshold { get; set; }
    }
}
