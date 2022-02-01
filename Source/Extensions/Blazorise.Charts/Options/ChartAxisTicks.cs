#region Using directives
using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Common tick options to all axes.
    /// </summary>
    public class ChartAxisTicks
    {
        /// <summary>
        /// Color of label backdrops.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> BackdropColor { get; set; } = "rgba(255, 255, 255, 0.75)";

        /// <summary>
        /// Padding of label backdrop. See <see href="https://www.chartjs.org/docs/3.6.2/general/padding.html">Padding</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object BackdropPadding { get; set; } = 2;

        /// <summary>
        /// Defines the Expression which will be converted to JavaScript as a string representation of the tick value as it should be displayed on the chart.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( LambdaConverter<Func<double, int, double[], string>> ) )]
        public Expression<Func<double, int, double[], string>> Callback { get; set; }

        /// <summary>
        /// If true, show tick marks.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Display { get; set; } = true;

        /// <summary>
        /// Color of ticks.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> Color { get; set; }

        /// <summary>
        /// Font color for tick labels.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartFont Font { get; set; }

        /// <summary>
        /// Major ticks configuration. Omitted options are inherited from options above.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartAxisMajorTick Major { get; set; }

        /// <summary>
        /// Sets the offset of the tick labels from the axis.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Padding { get; set; } = 3;

        /// <summary>
        /// If true, draw a background behind the tick labels.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? ShowLabelBackdrop { get; set; } = false;

        /// <summary>
        /// The color of the stroke around the text.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> TextStrokeColor { get; set; }

        /// <summary>
        /// Stroke width around the text.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? TextStrokeWidth { get; set; } = 0;

        /// <summary>
        /// z-index of tick layer. Useful when ticks are drawn on chart area. Values &lt;= 0 are drawn under datasets, &gt; 0 on top.
        /// </summary>
        [JsonPropertyName( "y" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Z { get; set; } = 0;
    }
}
