#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <remarks>
    /// Defaults from https://www.chartjs.org/docs/latest/charts/radar.html#dataset-properties
    /// </remarks>
    public class RadarChartDataset<T> : ChartDataset<T>
    {
        public RadarChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "rgba(0, 0, 0, 0.1)",
            borderWidth: 3
        )
        {
            Type = "radar";
        }

        /// <summary>
        /// Cap style of the line. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineCap">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderCapStyle { get; set; } = "butt";

        /// <summary>
        /// Length and spacing of dashes. <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/setLineDash">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<int> BorderDash { get; set; } = new();

        /// <summary>
        /// Offset for line dashes. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineDashOffset">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? BorderDashOffset { get; set; } = 0f;

        /// <summary>
        /// Line joint style. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineJoin">MDN</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderJoinStyle { get; set; } = "miter";

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBackgroundColor { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string HoverBorderCapStyle { get; set; }

        /// <summary>
        /// The bar border color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBorderColor { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int[] HoverBorderDash { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderDashOffset { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string HoverBorderJoinStyle { get; set; }

        /// <summary>
        /// The bar border width when hovered (in pixels).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderWidth { get; set; }

        /// <summary>
        /// Fill the area under the line. See <see href="https://www.chartjs.org/docs/latest/charts/area.html">area charts.</see>.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Fill { get; set; } = false;

        /// <summary>
        /// Bezier curve tension of the line. Set to 0 to draw straight lines.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? Tension { get; set; } = 0f;

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
        public float? PointBorderWidth { get; set; } = 1f;

        /// <summary>
        /// The pixel size of the non-displayed point that reacts to mouse events.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? PointHitRadius { get; set; } = 1f;

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
        public float? PointRadius { get; set; } = 3f;

        /// <summary>
        /// The rotation of the point in degrees.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? PointRotation { get; set; } = 0f;

        /// <summary>
        /// Style of the point.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string PointStyle { get; set; } = "circle";

        /// <summary>
        /// If true, lines will be drawn between points with no or null data. If false, points with null data will create a break in the line.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object SpanGaps { get; set; }
    }
}
