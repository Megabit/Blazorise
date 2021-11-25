#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class LegendLabels
    {
        /// <summary>
        /// Default font color for all text.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string FontColor { get; set; } = "#666";

        /// <summary>
        /// Default font family for all text.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string FontFamily { get; set; } = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

        /// <summary>
        /// Default font size (in px) for text. Does not apply to radialLinear scale point labels.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? FontSize { get; set; } = 12;

        /// <summary>
        /// Default font style. Does not apply to tooltip title or footer. Does not apply to chart title.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string FontStyle { get; set; } = "normal";
    }
}
