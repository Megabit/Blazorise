#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Title Configuration.
    /// </summary>
    public class ChartTitle
    {
        /// <summary>
        /// Alignment of the title.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Align { get; set; } = "center";

        /// <summary>
        /// Color of the text.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> Color { get; set; }

        /// <summary>
        /// Is the legend title displayed.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Display { get; set; } = false;

        /// <summary>
        /// Marks that this box should take the full width/height of the canvas. If false, the box is sized and placed above/beside the chart area.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? FullSize { get; set; } = true;

        /// <summary>
        /// Position of title.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Position { get; set; } = "top";

        /// <summary>
        /// Font of the text.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartFont Font { get; set; } = new ChartFont { Weight = "bold" };

        /// <summary>
        /// Padding around the title.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object Padding { get; set; } = 10;

        /// <summary>
        /// Title text to display.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Text { get; set; }
    }
}
