#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Defines the chart transition options for Zoom.
/// </summary>
public class ChartZoomTransition
{
    /// <summary>
    /// Defines the zoom animation options.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAnimation Animation { get; set; }

}
