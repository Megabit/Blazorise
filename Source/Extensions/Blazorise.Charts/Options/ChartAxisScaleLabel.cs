#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Defines options for the scale title.
/// </summary>
public class ChartAxisScaleLabel
{
    /// <summary>
    /// If true, display the axis title.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// The text for the title. (i.e. "# of People" or "Response Choices").
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string LabelString { get; set; }

    /// <summary>
    /// Height of an individual line of text (https://developer.mozilla.org/en-US/docs/Web/CSS/line-height).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? LineHeight { get; set; }

    /// <summary>
    /// Font color for scale title.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string FontColor { get; set; }

    /// <summary>
    /// Font family for the scale title, follows CSS font-family options.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string FontFamily { get; set; }

    /// <summary>
    /// Font size for scale title.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? FontSize { get; set; }

    /// <summary>
    /// Font style for the scale title, follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string FontStyle { get; set; }

    /// <summary>
    /// Padding to apply around scale labels. Only top and bottom are implemented.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Padding { get; set; }
}