#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <remarks>
    /// Defaults values as per https://www.chartjs.org/docs/latest/charts/line.html#dataset-properties
    /// </remarks>
    public class LineChartDataset<T> : ChartDataset<T>
    {
        public LineChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "rgba(0, 0, 0, 0.1)",
            borderWidth: 3
        )
        {
            Type = "line";
            Stack = "line";
        }

        /// <summary>
        /// The ID of the group to which this dataset belongs to (when stacked, each group will be a separate stack). Defaults
        /// to dataset type.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Stack { get; set; }

        /// <summary>
        /// Cap style of the line. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineCap">MDN</see>.
        /// <list type="bullet">
        ///     <item>
        ///         <term>"butt"</term>
        ///         <description>The ends of lines are squared off at the endpoints. Default value.</description>
        ///     </item>
        ///     <item>
        ///         <term>"round"</term>
        ///         <description>The ends of lines are rounded.</description>
        ///     </item>
        ///     <item>
        ///         <term>"square"</term>
        ///         <description>The ends of lines are squared off by adding a box with an equal width and half the height of the line's thickness.</description>
        ///     </item>
        /// </list>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderCapStyle { get; set; } = "butt";

        /// <summary>
        /// Length and spacing of dashes. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/setLineDash">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<int> BorderDash { get; set; } = new();

        /// <summary>
        /// Offset for line dashes. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineDashOffset">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? BorderDashOffset { get; set; }

        /// <summary>
        /// Line joint style. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineJoin">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderJoinStyle { get; set; } = "miter";

        /// <summary>
        /// <para>
        ///     The following interpolation modes are supported.
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>default</term>
        ///         <description>The 'default' algorithm uses a custom weighted cubic interpolation, which produces pleasant curves for all types of datasets.</description>
        ///     </item>
        ///     <item>
        ///         <term>monotone</term>
        ///         <description>The 'monotone' algorithm is more suited to y = f(x) datasets: it preserves monotonicity (or piecewise monotonicity) of the dataset being interpolated, and ensures local extremums (if any) stay at input data points.</description>
        ///     </item>
        /// </list>
        /// <para>
        ///     If left untouched (undefined), the global options.elements.line.cubicInterpolationMode property is used.
        /// </para>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string CubicInterpolationMode { get; set; } = "default";

        /// <summary>
        /// Fill the area under the line. See <see href="https://www.chartjs.org/docs/latest/charts/area.html">area charts.</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Fill { get; set; } = true;

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBackgroundColor { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string HoverBorderCapStyle { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBorderColor { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int[] HoverBorderDash { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderDashOffset { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string HoverBorderJoinStyle { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderWidth { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string IndexAxis { get; set; }

        /// <summary>
        /// The fill color for points.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> PointBackgroundColor { get; set; }

        /// <summary>
        /// The border color for points.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> PointBorderColor { get; set; }

        /// <summary>
        /// The width of the point border in pixels.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? PointBorderWidth { get; set; } = 1;

        /// <summary>
        /// The pixel size of the non-displayed point that reacts to mouse events.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? PointHitRadius { get; set; } = 1;

        /// <summary>
        /// Point background color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> PointHoverBackgroundColor { get; set; }

        /// <summary>
        /// Point border color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> PointHoverBorderColor { get; set; }

        /// <summary>
        /// Border width of point when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? PointHoverBorderWidth { get; set; } = 1f;

        /// <summary>
        /// The radius of the point when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? PointHoverRadius { get; set; } = 4f;

        /// <summary>
        /// The radius of the point shape. If set to 0, the point is not rendered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? PointRadius { get; set; } = 3.0f;

        /// <summary>
        /// The radius of the point shape. If set to 0, the point is not rendered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? PointRotation { get; set; } = 0f;

        /// <summary>
        /// Style of the point.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string PointStyle { get; set; } = "circle";

        /// <summary>
        /// If false, the line is not drawn for this dataset.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? ShowLine { get; set; } = true;

        /// <summary>
        /// <para>
        /// Line segment styles can be overridden by scriptable options in the <c>segment</c> object. Currently all of the <c>border*</c> and <c>backgroundColor</c> options are supported. The segment styles are resolved for each section of the line between each point. <c>undefined</c> fallbacks to main line styles.
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>type</term>
        ///         <description>segment</description>
        ///     </item>
        ///     <item>
        ///         <term>p0</term>
        ///         <description>first point element</description>
        ///     </item>
        ///     <item>
        ///         <term>p1</term>
        ///         <description>second point element</description>
        ///     </item>
        ///     <item>
        ///         <term>p0DataIndex</term>
        ///         <description>index of first point in the data array</description>
        ///     </item>
        ///     <item>
        ///         <term>p1DataIndex</term>
        ///         <description>index of second point in the data array</description>
        ///     </item>
        ///     <item>
        ///         <term>datasetIndex</term>
        ///         <description>dataset index</description>
        ///     </item>
        /// </list>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object Segment { get; set; }

        /// <summary>
        /// If true, lines will be drawn between points with no or null data. If false, points with null data will
        /// create a break in the line. Can also be a number specifying the maximum gap length to span. The unit of the value depends on the scale used.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object SpanGaps { get; set; }

        /// <summary>
        /// <para>
        ///     If the line is shown as a <c>stepped</c> line.
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>false</term>
        ///         <description>No Step Interpolation (default)</description>
        ///     </item>
        ///     <item>
        ///         <term>true</term>
        ///         <description>Step-before Interpolation (eq. 'before')</description>
        ///     </item>
        ///     <item>
        ///         <term>"before"</term>
        ///         <description>Step-before Interpolation</description>
        ///     </item>
        ///     <item>
        ///         <term>"after"</term>
        ///         <description>Step-after Interpolation</description>
        ///     </item>
        ///     <item>
        ///         <term>"middle"</term>
        ///         <description>Step-middle Interpolation</description>
        ///     </item>
        /// </list>
        /// <para>
        ///     If the <c>stepped</c> value is set to anything other than false, <c>tension</c> will be ignored.
        /// </para>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object Stepped { get; set; } = false;

        /// <summary>
        /// Bezier curve tension of the line. Set to 0 to draw straightlines. This option is ignored if monotone cubic
        /// interpolation is used.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? Tension { get; set; } = 0f;

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
