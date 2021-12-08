#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Legend Label Configuration.
    /// </summary>
    public class ChartLegendLabel
    {
        /// <summary>
        /// Width of coloured box.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? BoxWidth { get; set; } = 40;

        /// <summary>
        /// Height of the coloured box.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? BoxHeight { get; set; } = 40;

        /// <summary>
        /// Color of label and the strikethrough.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> Color { get; set; }

        /// <summary>
        /// Font of the label.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartFont Font { get; set; }

        /// <summary>
        /// Padding between rows of colored boxes.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object Padding { get; set; } = 10;

        /// <summary>
        /// If specified, this style of point is used for the legend. Only used if usePointStyle is true.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string PointStyle { get; set; } = "circle";

        /// <summary>
        /// Horizontal alignment of the label text. Options are: 'left', 'right' or 'center'.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string TextAlign { get; set; } = "center";

        /// <summary>
        /// Label style will match corresponding point style (size is based on the minimum value between boxWidth and font.size).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? UsePointStyle { get; set; } = false;
    }
}
