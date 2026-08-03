#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Point annotation specific options.
/// </summary>
public class PointChartAnnotationOptions : ChartAnnotationOptions
{
    /// <summary>
    /// Shadow color cast behind the point marker.
    /// </summary>
    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    /// <summary>
    /// Outline thickness of the point marker.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Canvas marker shape used for the point.
    /// </summary>
    [JsonPropertyName( "pointStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Radius of the point marker in pixels.
    /// </summary>
    [JsonPropertyName( "radius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Radius { get; set; }

    /// <summary>
    /// Marker rotation around its coordinate.
    /// </summary>
    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Horizontal pixel adjustment after scale positioning.
    /// </summary>
    [JsonPropertyName( "xAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XAdjust { get; set; }

    /// <summary>
    /// Horizontal data coordinate of the point marker.
    /// </summary>
    [JsonPropertyName( "xValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XValue { get; set; }

    /// <summary>
    /// Vertical pixel adjustment after scale positioning.
    /// </summary>
    [JsonPropertyName( "yAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YAdjust { get; set; }

    /// <summary>
    /// Vertical data coordinate of the point marker.
    /// </summary>
    [JsonPropertyName( "yValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YValue { get; set; }
}