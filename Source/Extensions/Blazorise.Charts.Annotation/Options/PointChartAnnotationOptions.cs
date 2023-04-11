#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Point annotation specific options.
/// </summary>
public class PointChartAnnotationOptions : ChartAnnotationOptions
{
    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    [JsonPropertyName( "pointStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string PointStyle { get; set; }

    [JsonPropertyName( "radius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Radius { get; set; }

    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    [JsonPropertyName( "xAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XAdjust { get; set; }

    [JsonPropertyName( "xValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XValue { get; set; }

    [JsonPropertyName( "yAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YAdjust { get; set; }

    [JsonPropertyName( "yValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YValue { get; set; }
}