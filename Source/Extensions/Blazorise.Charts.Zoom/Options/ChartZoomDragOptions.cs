#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// The Chart Zoom plugin zoom drag.
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#drag-options</para>
/// </summary>
public class ChartZoomDragOptions
{
    /// <summary>
    /// Enable drag-to-zoom.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Fill color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Stroke color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Stroke width.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Minimal zoom distance required before actually applying zoom.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Threshold { get; set; }

    /// <summary>
    /// Modifier key required for drag-to-zoom.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ModifierKey { get; set; }
}