#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Defines the font styles.
    /// </summary>
    public class ChartFont
    {
        /// <summary>
        /// Default font family for all text, follows CSS font-family options.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Family { get; set; } = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

        /// <summary>
        /// Default font size (in px) for text. Does not apply to radialLinear scale point labels.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Size { get; set; } = 12;

        /// <summary>
        /// Default font style. Does not apply to tooltip title or footer. Does not apply to chart title. Follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Style { get; set; } = "normal";

        /// <summary>
        /// Default font weight (boldness). See <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/font-weight">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Weight { get; set; }

        /// <summary>
        /// Height of an individual line of text. See <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/line-height">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? LineHeight { get; set; } = 1.2d;
    }
}
