#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Polygon annotation specific options.
/// </summary>
public class PolygonChartAnnotationOptions : ChartAnnotationOptions
{
    /// <summary>
    /// Shadow color beneath the polygon interior.
    /// </summary>
    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    /// <summary>
    /// Canvas treatment for exposed border endpoints.
    /// </summary>
    [JsonPropertyName( "borderCapStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderCapStyle { get; set; }

    /// <summary>
    /// Canvas treatment where polygon edges connect.
    /// </summary>
    [JsonPropertyName( "borderJoinStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderJoinStyle { get; set; }

    /// <summary>
    /// Width of the polygon outline in pixels.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Styling applied to vertices of the polygon.
    /// </summary>
    [JsonPropertyName( "point" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Point { get; set; }

    /// <summary>
    /// Distance from the center to each polygon vertex.
    /// </summary>
    [JsonPropertyName( "radius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Radius { get; set; }

    /// <summary>
    /// Angular offset of the first polygon vertex.
    /// </summary>
    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Number of edges used to construct the polygon.
    /// </summary>
    [JsonPropertyName( "sides" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Sides { get; set; }

    /// <summary>
    /// Horizontal pixel shift from the resolved coordinate.
    /// </summary>
    [JsonPropertyName( "xAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XAdjust { get; set; }

    /// <summary>
    /// Horizontal data coordinate of the polygon center.
    /// </summary>
    [JsonPropertyName( "xValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XValue { get; set; }

    /// <summary>
    /// Vertical pixel shift from the resolved coordinate.
    /// </summary>
    [JsonPropertyName( "yAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YAdjust { get; set; }

    /// <summary>
    /// Vertical data coordinate of the polygon center.
    /// </summary>
    [JsonPropertyName( "yValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YValue { get; set; }
}