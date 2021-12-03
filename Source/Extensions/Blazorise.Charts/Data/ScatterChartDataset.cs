#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class ScatterChartDataset<T> : ChartDataset<T>
    {
        public ScatterChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "rgba(0, 0, 0, 0.1)",
            borderWidth: 3
        )
        {
            Type = "scatter";
            Stack = "scatter";
        }

        /// <summary>
        /// The ID of the group to which this dataset belongs to (when stacked, each group will be a separate stack). Defaults
        /// to dataset type.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Stack { get; set; }

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

    public struct ScatterChartPoint
    {
        public ScatterChartPoint( double? x, double? y )
        {
            X = x;
            Y = y;
        }

        [JsonPropertyName( "x" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? X { get; set; }

        [JsonPropertyName( "y" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Y { get; set; }
    }
}
