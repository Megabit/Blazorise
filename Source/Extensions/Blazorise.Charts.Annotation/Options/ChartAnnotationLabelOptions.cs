#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Presentation and positioning settings for text attached to an annotation.
/// </summary>
public class ChartAnnotationLabelOptions
{
    /// <summary>
    /// Should the scale range be adjusted if this annotation is out of range.
    /// </summary>
    [JsonPropertyName( "adjustScaleRange" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? AdjustScaleRange { get; set; }

    /// <summary>
    /// Fill painted behind the label content.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundColor { get; set; }

    /// <summary>
    /// Stroke color around the label boundary.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BorderColor { get; set; }

    /// <summary>
    /// Dash and gap lengths used for the label outline.
    /// </summary>
    [JsonPropertyName( "borderDash" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double?[] BorderDash { get; set; }

    /// <summary>
    /// Offset into the outline's dash sequence.
    /// </summary>
    [JsonPropertyName( "borderDashOffset" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderDashOffset { get; set; }

    /// <summary>
    /// Color of the shadow behind the label background.
    /// </summary>
    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    /// <summary>
    /// Canvas cap style for open label border paths.
    /// </summary>
    [JsonPropertyName( "borderCapStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderCapStyle { get; set; }

    /// <summary>
    /// Canvas join style at label border corners.
    /// </summary>
    [JsonPropertyName( "borderJoinStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderJoinStyle { get; set; }

    /// <summary>
    /// If this value is a number, it is applied to all corners of the rectangle (topLeft, topRight, bottomLeft, bottomRight). If this value is an object, the topLeft property defines the top-left corners border radius. Similarly, the topRight, bottomLeft, and bottomRight properties can also be specified. Omitted corners have radius of 0.
    /// </summary>
    [JsonPropertyName( "borderRadius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object BorderRadius { get; set; }

    /// <summary>
    /// Thickness of the label outline in pixels.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Connector-line settings between the label and its annotation.
    /// </summary>
    [JsonPropertyName( "callout" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Callout { get; set; }

    /// <summary>
    /// Foreground color of the label text.
    /// </summary>
    [JsonPropertyName( "color" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? Color { get; set; }

    /// <summary>
    /// One or more text values rendered inside the label.
    /// </summary>
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

    /// <summary>
    /// Typography for each line of label content.
    /// </summary>
    [JsonPropertyName( "font" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<ChartFont> ) )]
    public IndexableOption<ChartFont> Font { get; set; }

    /// <summary>
    /// Explicit label height expressed as a CSS-compatible value.
    /// </summary>
    [JsonPropertyName( "height" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Height { get; set; }

    /// <summary>
    /// Whether the label participates in initial annotation animation.
    /// </summary>
    [JsonPropertyName( "init" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Init { get; set; }

    /// <summary>
    /// Identifies a unique id for the annotation and it will be stored in the element context. When the annotations are defined by an object, the id is automatically set using the key used to store the annotations in the object. When the annotations are configured by an array, the id, passed by this option in the annotation, will be used.
    /// </summary>
    [JsonPropertyName( "id" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Id { get; set; }

    /// <summary>
    /// Alpha applied to the entire label.
    /// </summary>
    [JsonPropertyName( "opacity" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Opacity { get; set; }

    /// <summary>
    /// Interior spacing around the label content.
    /// </summary>
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

    /// <summary>
    /// Clockwise rotation of label content in degrees.
    /// </summary>
    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Blur radius of the label shadow.
    /// </summary>
    [JsonPropertyName( "shadowBlur" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowBlur { get; set; }

    /// <summary>
    /// Horizontal distance between the label and its shadow.
    /// </summary>
    [JsonPropertyName( "shadowOffsetX" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowOffsetX { get; set; }

    /// <summary>
    /// Vertical distance between the label and its shadow.
    /// </summary>
    [JsonPropertyName( "shadowOffsetY" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? ShadowOffsetY { get; set; }

    /// <summary>
    /// Maximum horizontal coordinate used when the label owns its bounds.
    /// </summary>
    [JsonPropertyName( "xMax" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XMax { get; set; }

    /// <summary>
    /// Minimum horizontal coordinate used when the label owns its bounds.
    /// </summary>
    [JsonPropertyName( "xMin" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XMin { get; set; }

    /// <summary>
    /// Horizontal scale used to interpret label coordinates.
    /// </summary>
    [JsonPropertyName( "xScaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XScaleID { get; set; }

    /// <summary>
    /// Maximum vertical coordinate used when the label owns its bounds.
    /// </summary>
    [JsonPropertyName( "yMax" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YMax { get; set; }

    /// <summary>
    /// Minimum vertical coordinate used when the label owns its bounds.
    /// </summary>
    [JsonPropertyName( "yMin" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YMin { get; set; }

    /// <summary>
    /// Vertical scale used to interpret label coordinates.
    /// </summary>
    [JsonPropertyName( "yScaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YScaleID { get; set; }

    /// <summary>
    /// Stacking order among overlapping chart elements.
    /// </summary>
    [JsonPropertyName( "z" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Z { get; set; }

    /// <summary>
    /// Horizontal alignment of multiline text within the label.
    /// </summary>
    [JsonPropertyName( "textAlign" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string TextAlign { get; set; }

    /// <summary>
    /// Color used to outline text glyphs.
    /// </summary>
    [JsonPropertyName( "textStrokeColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? TextStrokeColor { get; set; }

    /// <summary>
    /// Width of the text glyph outline in pixels.
    /// </summary>
    [JsonPropertyName( "textStrokeWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? TextStrokeWidth { get; set; }

    /// <summary>
    /// Explicit label width expressed as a CSS-compatible value.
    /// </summary>
    [JsonPropertyName( "width" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Width { get; set; }

    /// <summary>
    /// Final horizontal pixel adjustment after positioning.
    /// </summary>
    [JsonPropertyName( "xAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? XAdjust { get; set; }

    /// <summary>
    /// Horizontal data value used to place the label.
    /// </summary>
    [JsonPropertyName( "xValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XValue { get; set; }

    /// <summary>
    /// Final vertical pixel adjustment after positioning.
    /// </summary>
    [JsonPropertyName( "yAdjust" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? YAdjust { get; set; }

    /// <summary>
    /// Vertical data value used to place the label.
    /// </summary>
    [JsonPropertyName( "yValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YValue { get; set; }
}