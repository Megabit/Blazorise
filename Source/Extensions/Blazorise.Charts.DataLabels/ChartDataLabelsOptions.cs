#region Using directives
using System;
using System.Text.Json.Serialization;
using System.Linq.Expressions;
using Blazorise.Charts;
#endregion

namespace Blazorise.Charts.DataLabels;

public class ChartDataLabelsOptions
{
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> Align { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> Anchor { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> BackgroundColor { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> BorderColor { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> BorderRadius { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> BorderWidth { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<bool?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<bool?, ScriptableOptionsContext> Clamp { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<bool?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<bool?, ScriptableOptionsContext> Clip { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> Color { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<object, ScriptableOptionsContext> ) )]
    public ScriptableOptions<object, ScriptableOptionsContext> Display { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont Font { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartMathFormatter? Formatter { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableValueBasedOptionsConverter<string, object, ScriptableOptionsContext> ) )]
    public ScriptableValueBasedOptions<string, object, ScriptableOptionsContext> ScriptableFormatter { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Labels { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Listeners { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> Offset { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> Opacity { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartPadding Padding { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> Rotation { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> TextAlign { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> TextStrokeColor { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> TextStrokeWidth { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<double?, ScriptableOptionsContext> ) )]
    public ScriptableOptions<double?, ScriptableOptionsContext> TextShadowBlur { get; set; }

    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ScriptableOptionsConverter<string, ScriptableOptionsContext> ) )]
    public ScriptableOptions<string, ScriptableOptionsContext> TextShadowColor { get; set; }
}