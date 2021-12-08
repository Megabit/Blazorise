#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <remarks>
    /// Defaults as per https://www.chartjs.org/docs/latest/charts/bar.html#dataset-properties
    /// </remarks>
    public class BarChartDataset<T> : ChartDataset<T>
    {
        public BarChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "rgba(0, 0, 0, 0.1)",
            borderWidth: 0
        )
        {
            Type = "bar";
            Stack = "bar";
        }

        /// <summary>
        /// Base value for the bar in data units along the value axis. If not set, defaults to the value axis base value.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Base { get; set; }

        /// <summary>
        /// Percent (0-1) of the available width each bar should be within the category width. 1.0 will take the whole
        /// category width and put the bars right next to each other.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? BarPercentage { get; set; }

        /// <summary>
        ///     <para>
        ///         If this value is a number, it is applied to the width of each bar, in pixels. When this is enforced, barPercentage and categoryPercentage are ignored.
        ///     </para>
        ///     <para>
        ///         If set to 'flex', the base sample widths are calculated automatically based on the previous and following samples so that they take the full available widths without overlap. Then, bars are sized using barPercentage and categoryPercentage. There is no gap when the percentage options are 1. This mode generates bars with different widths when data are not evenly spaced.
        ///     </para>
        ///     <para>
        ///         If not set (default), the base sample widths are calculated using the smallest interval that prevents bar overlapping, and bars are sized using barPercentage and categoryPercentage. This mode always generates bars equally sized.
        ///     </para>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BarThickness { get; set; }

        /// <summary>
        /// This setting is used to avoid drawing the bar stroke at the base of the fill, or disable the border radius.
        /// In general, this does not need to be changed except when creating chart types that derive from a bar chart.
        /// <list type="bullet">
        ///     <item>
        ///         <term>"start"</term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term>"end"</term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term>"middle"</term>
        ///         <description>(only valid on stacked bars: the borders between bars are skipped)</description>
        ///     </item>
        ///     <item>
        ///         <term>"bottom"</term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term>"left"</term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term>"top"</term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term>"right"</term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term>false</term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderSkipped { get; set; } = "start";

        /// <summary>
        /// If this value is a number, it is applied to all corners of the rectangle (topLeft, topRight, bottomLeft, bottomRight), except corners touching the borderSkipped. If this value is an object, the topLeft property defines the top-left corners border radius. Similarly, the topRight, bottomLeft, and bottomRight properties can also be specified. Omitted corners and those touching the borderSkipped are skipped. For example if the top border is skipped, the border radius for the corners topLeft and topRight will be skipped as well.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? BorderRadius { get; set; } = 0f;

        /// <summary>
        /// Percent (0-1) of the available width each category should be within the sample width.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? CategoryPercentage { get; set; } = 0.8f;

        /// <summary>
        /// Should the bars be grouped on index axis. When true, all the datasets at same index value will be placed next
        /// to each other centering on that index value. When false, each bar is placed on its actual index-axis value.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Grouped { get; set; }

        /// <summary>
        /// The bar background color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBackgroundColor { get; set; }

        /// <summary>
        /// The bar border color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBorderColor { get; set; }

        /// <summary>
        /// The bar border width when hovered (in pixels).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderWidth { get; set; } = 1;

        /// <summary>
        /// The bar border radius when hovered (in pixels).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderRadius { get; set; } = 0;

        /// <summary>
        /// The base axis of the dataset. 'x' for vertical bars and 'y' for horizontal bars.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string IndexAxis { get; set; } = "x";

        /// <summary>
        /// This option can be used to inflate the rects that are used to draw the bars. This can be used to hide artifacts
        /// between bars when barPercentage(#barpercentage) * categoryPercentage(#categorypercentage) is 1. The default
        /// value 'auto' should work in most cases.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object InflateAmount { get; set; } = "auto";

        /// <summary>
        /// Set this to ensure that bars are not sized thicker than this.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string MaxBarThickness { get; set; }

        /// <summary>
        /// Set this to ensure that bars have a minimum length in pixels.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? MinBarLength { get; set; }

        /// <summary>
        /// Style of the point for legend. https://www.chartjs.org/docs/latest/configuration/elements.html#point-styles
        /// </summary>    
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string PointStyle { get; set; } = "circle";

        /// <summary>
        /// If true, null or undefined values will not be used for spacing calculations when determining bar size.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? SkipNull { get; set; }

        /// <summary>
        /// The ID of the group to which this dataset belongs to (when stacked, each group will be a separate stack). Defaults
        /// to dataset type.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Stack { get; set; }

        /// <summary>
        /// The ID of the x-axis to plot this dataset on.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string XAxisID { get; set; }

        /// <summary>
        /// The ID of the y-axis to plot this dataset on.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string YAxisID { get; set; }
    }
}
