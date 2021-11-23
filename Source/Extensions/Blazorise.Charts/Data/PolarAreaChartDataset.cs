#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <remarks>
    /// Defaults as per https://www.chartjs.org/docs/latest/charts/polar.html#dataset-properties
    /// </remarks>
    [DataContract]
    public class PolarAreaChartDataset<T> : ChartDataset<T>
    {
        public PolarAreaChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "rgba(0, 0, 0, 0.1)",
            borderWidth: 2
        )
        {
            Type = "polarArea";
        }

        /// <summary>
        /// Defines the border alignment.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderAlign { get; set; } = "center";

        /// <summary>
        /// The fill colour of the arcs when hovered.
        /// </summary>
        /// <remarks>Default as per https://www.chartjs.org/docs/latest/configuration/elements.html#arc-configuration </remarks>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBackgroundColor { get; set; }

        /// <summary>
        /// The stroke colour of the arcs when hovered.
        /// </summary>
        /// <remarks>Default as per https://www.chartjs.org/docs/latest/configuration/elements.html#arc-configuration </remarks>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBorderColor { get; set; }

        /// <summary>
        /// The stroke width of the arcs when hovered.
        /// </summary>
        /// <remarks>Default as per https://www.chartjs.org/docs/latest/configuration/elements.html#arc-configuration </remarks>
        [DataMember]
        public int HoverBorderWidth { get; set; }
    }
}
