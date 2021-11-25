#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class LineChartOptions : ChartOptions
    {
        /// <summary>
        /// If false, the lines between points are not drawn.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? ShowLines { get; set; } = true;

        /// <summary>
        /// If false, NaN data causes a break in the line.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? SpanGaps { get; set; } = false;
    }
}