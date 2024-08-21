#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// To configure which events trigger chart interactions.
/// </summary>
public class ChartInteraction
{
    /// <summary>
    /// Sets which elements appear in the interaction.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Mode { get; set; }

    /// <summary>
    /// If true, the interaction mode only applies when the mouse position intersects an item on the chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Intersect { get; set; }

    /// <summary>
    /// Can be set to 'x', 'y', or 'xy' to define which directions are used in calculating distances. Defaults to 'x' for 'index' mode and 'xy' in dataset and 'nearest' modes.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Axis { get; set; }
}