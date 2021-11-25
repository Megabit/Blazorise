#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class BarChartOptions : ChartOptions
    {
        /// <summary>
        /// Percent (0-1) of the available width each bar should be within the category width. 1.0 will take the whole category width and put the bars right next to each other.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? BarPercentage { get; set; } = 0.9f;

        /// <summary>
        /// Percent (0-1) of the available width each category should be within the sample width. 
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? CategoryPercentage { get; set; } = 0.8f;
    }
}
