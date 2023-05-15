#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

public class ChartAnnotationLabelOptions
{
    /// <summary>
    /// Should the scale range be adjusted if this annotation is out of range.
    /// </summary>
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
    public double?[] BorderDash { get; set; }

    [JsonPropertyName( "borderDashOffset" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderDashOffset { get; set; }

    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    [JsonPropertyName( "borderCapStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderCapStyle { get; set; }

    [JsonPropertyName( "borderJoinStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderJoinStyle { get; set; }

    /// <summary>
    /// If this value is a number, it is applied to all corners of the rectangle (topLeft, topRight, bottomLeft, bottomRight). If this value is an object, the topLeft property defines the top-left corners border radius. Similarly, the topRight, bottomLeft, and bottomRight properties can also be specified. Omitted corners have radius of 0.
    /// </summary>
    [JsonPropertyName( "borderRadius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object BorderRadius { get; set; }

    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    [JsonPropertyName( "callout" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Callout { get; set; }

    [JsonPropertyName( "color" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? Color { get; set; }

    [JsonPropertyName( "content" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<string> ) )]
    public IndexableOption<string> Content { get; set; }

    /// <summary>
    /// Whether or not this annotation is visible.
    /// </summary>
    [JsonPropertyName( "display" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// The drawTime option for an annotation determines where in the chart lifecycle the drawing occurs. Four potential options are available:
    /// </summary>
    [JsonPropertyName( "drawTime" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string DrawTime { get; set; }

    [JsonPropertyName( "font" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<ChartFont> ) )]
    public IndexableOption<ChartFont> Font { get; set; }

    [JsonPropertyName( "height" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Height { get; set; }

    [JsonPropertyName( "init" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Init { get; set; }

    /// <summary>
    /// Identifies a unique id for the annotation and it will be stored in the element context. When the annotations are defined by an object, the id is automatically set using the key used to store the annotations in the object. When the annotations are configured by an array, the id, passed by this option in the annotation, will be used.
    /// </summary>
    [JsonPropertyName( "id" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Id { get; set; }

    [JsonPropertyName( "opacity" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Opacity { get; set; }

    [JsonPropertyName( "padding" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartPadding Padding { get; set; }

    /// <summary>
    /// <para>
    /// A position can be set in 2 different values types:
    /// </para>
    /// <list type="number">
    /// <item>'start', 'center', 'end' which are defining where the label will be located</item>
    /// <item>a string, in percentage format 'number%', is representing the percentage on the size where the label will be located</item>
    /// </list>
    /// <para>
    /// If this value is a string (possible options are 'start', 'center', 'end' or a string in percentage format), it is applied to vertical and horizontal position in the box.
    /// </para>
    /// <para>
    /// If this value is an object, the x property defines the horizontal alignment in the label, with respect to the selected point. Similarly, the y property defines the vertical alignment in the label, with respect to the selected point. Possible options for both properties are 'start', 'center', 'end', a string in percentage format. Omitted property have value of the default, 'center'.
    /// </para>
    /// </summary>
    [JsonPropertyName( "position" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Position { get; set; }

    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

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

    [JsonPropertyName( "textAlign" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string TextAlign { get; set; }

    [JsonPropertyName( "textStrokeColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? TextStrokeColor { get; set; }

    [JsonPropertyName( "textStrokeWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? TextStrokeWidth { get; set; }

    [JsonPropertyName( "width" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Width { get; set; }

    [JsonPropertyName( "xAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XAdjust { get; set; }

    [JsonPropertyName( "xValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XValue { get; set; }

    [JsonPropertyName( "yAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YAdjust { get; set; }

    [JsonPropertyName( "yValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YValue { get; set; }
}