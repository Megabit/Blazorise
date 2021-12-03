#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Defines the chart animation options.
    /// </summary>
    public class ChartAnimation
    {
        /// <summary>
        /// The number of milliseconds an animation takes.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Duration { get; set; } = 1000;

        /// <summary>
        /// Easing function to use. <see href="https://www.chartjs.org/docs/latest/configuration/animations.html#easing">more...</see>
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Easing { get; set; } = "easeOutQuart";

        /// <summary>
        /// Delay before starting the animations.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Delay { get; set; }

        /// <summary>
        /// If set to true, the animations loop endlessly.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Loop { get; set; }
    }
}
