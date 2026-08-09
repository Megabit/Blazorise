#region Using directives
using System;
using System.Text.Json.Serialization;
using System.Linq.Expressions;
using Blazorise.Charts;
#endregion

namespace Blazorise.Charts.DataLabels;

/// <summary>
/// Controls how the Chart.js data-labels plugin positions and paints values.
/// </summary>
public class ChartDataLabelsOptions
{
    /// <summary>
    /// Label alignment relative to its anchor point.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> Align { get; set; }

    /// <summary>
    /// Element boundary used as the label's positioning origin.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> Anchor { get; set; }

    /// <summary>
    /// Fill painted behind each label.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> BackgroundColor { get; set; }

    /// <summary>
    /// Outline color surrounding the label box.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> BorderColor { get; set; }

    /// <summary>
    /// Corner radius of the label background.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> BorderRadius { get; set; }

    /// <summary>
    /// Thickness of the label outline in pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> BorderWidth { get; set; }

    /// <summary>
    /// Whether positioning is constrained to the visible portion of an element.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<bool?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<bool?, ScriptableOptionsContext> Clamp { get; set; }

    /// <summary>
    /// Whether label pixels outside the chart area are clipped.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<bool?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<bool?, ScriptableOptionsContext> Clip { get; set; }

    /// <summary>
    /// Foreground color used to draw label text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> Color { get; set; }

    /// <summary>
    /// Visibility rule evaluated for each data point.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<object, ScriptableOptionsContext> ) )]
    public ScriptableOptions<object, ScriptableOptionsContext> Display { get; set; }

    /// <summary>
    /// Typography applied to the rendered value.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont Font { get; set; }

    /// <summary>
    /// Callback that converts a chart value into label text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartMathFormatter? Formatter { get; set; }

    /// <summary>
    /// Named child-label configurations for rendering multiple labels per value.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Labels { get; set; }

    /// <summary>
    /// Pointer-event handlers registered for rendered labels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Listeners { get; set; }

    /// <summary>
    /// Distance in pixels between a label and its anchor.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> Offset { get; set; }

    /// <summary>
    /// Alpha applied to the complete label rendering.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> Opacity { get; set; }

    /// <summary>
    /// Space between the text and label boundary.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartPadding Padding { get; set; }

    /// <summary>
    /// Clockwise text rotation in degrees.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> Rotation { get; set; }

    /// <summary>
    /// Horizontal alignment used for multiline label text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> TextAlign { get; set; }

    /// <summary>
    /// Color of the outline painted around text glyphs.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> TextStrokeColor { get; set; }

    /// <summary>
    /// Width in pixels of the text glyph outline.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> TextStrokeWidth { get; set; }

    /// <summary>
    /// Blur radius applied to the text shadow.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> TextShadowBlur { get; set; }

    /// <summary>
    /// Color cast behind label text as a shadow.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> TextShadowColor { get; set; }
}