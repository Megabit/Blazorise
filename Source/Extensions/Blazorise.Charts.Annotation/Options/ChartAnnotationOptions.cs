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
    /// <summary>
    /// Shape discriminator understood by the Chart.js annotation plugin.
    /// </summary>
    [JsonPropertyName( "type" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Type { get; set; }

    /// <summary>
    /// Whether an out-of-range annotation expands its associated scales.
    /// </summary>
    [JsonPropertyName( "adjustScaleRange" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? AdjustScaleRange { get; set; }

    /// <summary>
    /// Fill painted inside the annotation geometry.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundColor { get; set; }

    /// <summary>
    /// Stroke color used around the annotation.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BorderColor { get; set; }

    /// <summary>
    /// Alternating dash and gap lengths for the border stroke.
    /// </summary>
    [JsonPropertyName( "borderDash" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public List<double> BorderDash { get; set; }

    /// <summary>
    /// Starting offset within the border dash pattern.
    /// </summary>
    [JsonPropertyName( "borderDashOffset" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderDashOffset { get; set; }

    /// <summary>
    /// Color cast by the border shadow.
    /// </summary>
    [JsonPropertyName( "borderShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BorderShadowColor { get; set; }

    /// <summary>
    /// Whether the annotation participates in rendering.
    /// </summary>
    [JsonPropertyName( "display" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// Chart lifecycle phase in which the annotation is painted.
    /// </summary>
    [JsonPropertyName( "drawTime" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string DrawTime { get; set; }

    /// <summary>
    /// Whether initial animation state is calculated for the annotation.
    /// </summary>
    [JsonPropertyName( "init" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Init { get; set; }

    /// <summary>
    /// Stable identifier stored in the annotation element context.
    /// </summary>
    [JsonPropertyName( "id" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Id { get; set; }

    /// <summary>
    /// Blur radius applied to the annotation shadow.
    /// </summary>
    [JsonPropertyName( "shadowBlur" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowBlur { get; set; }

    /// <summary>
    /// Horizontal displacement of the annotation shadow.
    /// </summary>
    [JsonPropertyName( "shadowOffsetX" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowOffsetX { get; set; }

    /// <summary>
    /// Vertical displacement of the annotation shadow.
    /// </summary>
    [JsonPropertyName( "shadowOffsetY" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowOffsetY { get; set; }

    /// <summary>
    /// Upper horizontal scale boundary occupied by the annotation.
    /// </summary>
    [JsonPropertyName( "xMax" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XMax { get; set; }

    /// <summary>
    /// Lower horizontal scale boundary occupied by the annotation.
    /// </summary>
    [JsonPropertyName( "xMin" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XMin { get; set; }

    /// <summary>
    /// Identifier of the horizontal scale used for coordinates.
    /// </summary>
    [JsonPropertyName( "xScaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XScaleID { get; set; }

    /// <summary>
    /// Upper vertical scale boundary occupied by the annotation.
    /// </summary>
    [JsonPropertyName( "yMax" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YMax { get; set; }

    /// <summary>
    /// Lower vertical scale boundary occupied by the annotation.
    /// </summary>
    [JsonPropertyName( "yMin" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YMin { get; set; }

    /// <summary>
    /// Identifier of the vertical scale used for coordinates.
    /// </summary>
    [JsonPropertyName( "yScaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YScaleID { get; set; }

    /// <summary>
    /// Rendering order relative to overlapping chart elements.
    /// </summary>
    [JsonPropertyName( "z" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Z { get; set; }
}