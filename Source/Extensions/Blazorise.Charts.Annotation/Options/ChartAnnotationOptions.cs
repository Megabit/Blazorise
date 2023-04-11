#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Common options to all annotations.
/// </summary>
[JsonConverter( typeof( ChartAnnotationOptionsConverter ) )]
public abstract class ChartAnnotationOptions
{
    [JsonPropertyName( "type" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Type { get; set; }

    [JsonPropertyName( "adjustScaleRange" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? AdjustScaleRange { get; set; }

    [JsonPropertyName( "backgroundColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundColor { get; set; }

    [JsonPropertyName( "borderColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BorderColor { get; set; }

    [JsonPropertyName( "borderDash" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<double> BorderDash { get; set; }

    [JsonPropertyName( "borderDashOffset" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderDashOffset { get; set; }

    [JsonPropertyName( "borderShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BorderShadowColor { get; set; }

    [JsonPropertyName( "display" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    [JsonPropertyName( "drawTime" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string DrawTime { get; set; }

    [JsonPropertyName( "init" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Init { get; set; }

    [JsonPropertyName( "id" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Id { get; set; }

    [JsonPropertyName( "shadowBlur" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowBlur { get; set; }

    [JsonPropertyName( "shadowOffsetX" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowOffsetX { get; set; }

    [JsonPropertyName( "shadowOffsetY" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowOffsetY { get; set; }

    [JsonPropertyName( "xMax" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XMax { get; set; }

    [JsonPropertyName( "xMin" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XMin { get; set; }

    [JsonPropertyName( "xScaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XScaleID { get; set; }

    [JsonPropertyName( "yMax" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YMax { get; set; }

    [JsonPropertyName( "yMin" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YMin { get; set; }

    [JsonPropertyName( "yScaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YScaleID { get; set; }

    [JsonPropertyName( "z" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Z { get; set; }
}