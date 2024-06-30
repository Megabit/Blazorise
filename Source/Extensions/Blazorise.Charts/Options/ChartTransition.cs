#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Defines the chart transition options.
/// </summary>
public class ChartTransition
{
    /// <summary>
    /// The number of milliseconds an animation takes.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomTransition Zoom { get; set; }

}
