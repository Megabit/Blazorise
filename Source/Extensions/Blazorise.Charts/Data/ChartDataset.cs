#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Base class for the chart dataset.
    /// </summary>
    public class ChartDataset<T>
    {
        public ChartDataset() { }

        protected ChartDataset(
            string label,
            List<string> backgroundColor,
            List<string> borderColor,
            int borderWidth
        )
        {
            Label = label;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
        }

        protected ChartDataset(
            string label,
            IndexableOption<object> backgroundColor,
            IndexableOption<object> borderColor,
            int borderWidth
        )
        {
            Label = label;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
        }

        /// <summary>
        /// Defines the dataset display name.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Label { get; set; }

        /// <summary>
        /// List of data items.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<T> Data { get; set; }

        /// <summary>
        ///List of background colors for each of the data items.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> BackgroundColor { get; set; }

        /// <summary>
        /// List of border colors for each of the data items.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
        public IndexableOption<object> BorderColor { get; set; }

        /// <summary>
        /// Defines the border width.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? BorderWidth { get; set; } = 1;

        /// <summary>
        /// Defines the type of a chart dataset.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Type { get; set; }

        /// <summary>
        /// How to clip relative to chartArea. Positive value allows overflow, negative value clips that many pixels
        /// inside chartArea. 0 = clip at chartArea. Clipping can also be configured per side: clip: {left: 5, top: false,
        /// right: -2, bottom: 0}
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public object Clip { get; set; }

        /// <summary>
        /// The drawing order of dataset. Also affects order for stacking, tooltip and legend.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Order { get; set; } = 0;

        /// <summary>
        /// Configure the visibility of the dataset. Using <c>Hidden = true</c> will hide the dataset from being rendered in the Chart.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Hidden { get; set; } = false;
    }
}
