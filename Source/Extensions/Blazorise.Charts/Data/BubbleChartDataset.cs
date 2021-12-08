#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class BubbleChartDataset<T> : ChartDataset<T>
    {
        public BubbleChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "rgba(0, 0, 0, 0.1)",
            borderWidth: 3
        )
        {
            Type = "bubble";
        }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBackgroundColor { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBorderColor { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderWidth { get; set; } = 1;

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverRadius { get; set; } = 4;

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HitRadius { get; set; } = 1;

        /// <summary>
        /// Style of the point.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string PointStyle { get; set; } = "circle";

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Rotation { get; set; } = 0;

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Radius { get; set; } = 3;
    }

    public struct BubbleChartPoint
    {
        public BubbleChartPoint( double? x, double? y, double? r )
        {
            X = x;
            Y = y;
            R = r;
        }

        [JsonPropertyName( "x" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? X { get; set; }

        [JsonPropertyName( "y" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? Y { get; set; }

        [JsonPropertyName( "r" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? R { get; set; }
    }
}
