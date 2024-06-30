#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// The Chart Zoom plugin zoom wheel 
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#wheel-options</para>
/// </summary>
public class ChartZoomWheel
{
    /// <summary>
    /// Enable zooming via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Factor of zoom speed via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Speed { get; set; }

    /// <summary>
    /// Modifier key required for zooming via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ModifierKey { get; set; }
}
