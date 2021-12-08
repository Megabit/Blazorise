#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <remarks>
    /// Defaults as per https://www.chartjs.org/docs/latest/charts/doughnut.html#dataset-properties
    /// </remarks>
    public class PieChartDataset<T> : ChartDataset<T>
    {
        public PieChartDataset() : base(
            label: string.Empty,
            backgroundColor: "rgba(0, 0, 0, 0.1)",
            borderColor: "#fff",
            borderWidth: 2
        )
        {
            Type = "pie";
        }

        /// <summary>
        /// <para>
        ///     The following values are supported for <c>borderAlign</c>.
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>center</term>
        ///         <description>The borders of arcs next to each other will overlap (default).</description>
        ///     </item>
        ///     <item>
        ///         <term>inner</term>
        ///         <description>It is guaranteed that all borders will not overlap.</description>
        ///     </item>
        /// </list>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string BorderAlign { get; set; } = "center";

        /// <summary>
        /// If this value is a number, it is applied to all corners of the arc (outerStart, outerEnd, innerStart, innerRight). If this value is an object, the outerStart property defines the outer-start corner's border radius. Similarly, the outerEnd, innerStart, and innerEnd properties can also be specified.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object BorderRadius { get; set; } = 0f;

        /// <summary>
        /// Per-dataset override for the sweep that the arcs cover.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? Circumference { get; set; }

        /// <summary>
        /// Arc background color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBackgroundColor { get; set; }

        /// <summary>
        /// Arc border color when hovered.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> HoverBorderColor { get; set; }

        /// <summary>
        /// Arc border width when hovered (in pixels).
        /// </summary>
        /// <remarks>Default as per https://www.chartjs.org/docs/latest/configuration/elements.html#arc-configuration </remarks>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverBorderWidth { get; set; } = 0;

        /// <summary>
        /// Arc offset when hovered (in pixels).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? HoverOffset { get; set; } = 0;

        /// <summary>
        /// Arc offset (in pixels).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Offset { get; set; } = 0;

        /// <summary>
        /// Per-dataset override for the starting angle to draw arcs from.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Rotation { get; set; }

        /// <summary>
        /// Fixed arc offset (in pixels). Similar to offset but applies to all arcs.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Spacing { get; set; } = 0;

        /// <summary>
        /// The relative thickness of the dataset. Providing a value for weight will cause the pie or doughnut dataset
        /// to be drawn with a thickness relative to the sum of all the dataset weight values.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Weight { get; set; } = 1;
    }
}
