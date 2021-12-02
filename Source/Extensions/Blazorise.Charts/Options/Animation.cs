#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Defines the chart animation options.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The number of milliseconds an animation takes.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Duration { get; set; } = 1000;

        /// <summary>
        /// Easing function to use (https://www.chartjs.org/docs/latest/configuration/animations.html#easing).
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Easing { get; set; } = "easeOutQuart";
    }
}
