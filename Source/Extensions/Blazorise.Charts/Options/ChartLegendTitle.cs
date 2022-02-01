#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Legend Title Configuration.
    /// </summary>
    public class ChartLegendTitle
    {
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
        /// Font of the text.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartFont Font { get; set; }

        /// <summary>
        /// Padding around the title.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object Padding { get; set; } = 0;

        /// <summary>
        /// The string title.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Text { get; set; }
    }
}
