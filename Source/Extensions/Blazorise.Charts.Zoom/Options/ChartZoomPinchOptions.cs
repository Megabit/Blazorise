#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// The Chart Zoom plugin zoom pinch.
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#pinch-options</para>
/// </summary>
public class ChartZoomPinchOptions
{
    /// <summary>
    /// Enable zooming via pinch.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }
}
