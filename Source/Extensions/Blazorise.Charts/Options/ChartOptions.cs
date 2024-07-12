#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Chart options configuration.
/// </summary>
public class ChartOptions
{
    /// <summary>
    /// Configuration for chart scales.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartScales Scales { get; set; }

    /// <summary>
    /// Configuration for chart animation.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAnimation Animation { get; set; }

    /// <summary>
    /// Configuration for chart plugins.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartPlugins Plugins { get; set; }

    /// <summary>
    /// Configuration for chart interactions.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartInteraction Interaction { get; set; }

    /// <summary>
    /// Configuration for chart parsing.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartParsing Parsing { get; set; }

    /// <summary>
    /// Configuration for chart elements.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartElements Elements { get; set; }

    /// <summary>
    /// Configuration for chart layout.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartLayout Layout { get; set; }

    /// <summary>
    /// Index axis for the chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string IndexAxis { get; set; }

    /// <summary>
    /// Resizes the chart canvas when its container does.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Responsive { get; set; }

    /// <summary>
    /// Maintain the original canvas aspect ratio (width / height) when resizing.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? MaintainAspectRatio { get; set; }

    /// <summary>
    /// Canvas aspect ratio (i.e. width / height, a value of 1 representing a square canvas).
    /// Note that this option is ignored if the height is explicitly defined either as attribute or via the style.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? AspectRatio { get; set; }

    /// <summary>
    /// Delay the resize update by given amount of milliseconds. This can ease the resize process by debouncing update of the elements.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? ResizeDelay { get; set; }

    /// <summary>
    /// A string with a BCP 47 language tag, leveraging on <Anchor To="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat/NumberFormat" />
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Locale { get; set; }
}
