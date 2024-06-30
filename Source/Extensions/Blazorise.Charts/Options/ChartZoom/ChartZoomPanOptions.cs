#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// The Chart Zoom plugin pan options
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#pan-options</para>
/// </summary>
public class ChartZoomPanOptions
{
    /// <summary>
    /// Enable panning
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Allowed panning directions
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Mode { get; set; }

    /// <summary>
    /// Modifier key required for panning with mouse
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ModifierKey { get; set; }

    /// <summary>
    /// Enable panning over a scale for that axis (regardless of mode)
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ScaleMode { get; set; }

    /// <summary>
    /// Minimal pan distance required before actually applying pan
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Threshold { get; set; }
}
