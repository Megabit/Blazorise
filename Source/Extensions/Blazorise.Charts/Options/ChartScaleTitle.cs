#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Scale title Configuration.
/// </summary>
public class ChartScaleTitle
{
    /// <summary>
    /// If true, display the axis title.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// Alignment of the axis title. Possible options are 'start', 'center' and 'end'.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Align { get; set; }

    /// <summary>
    /// The text for the title. (i.e. "# of People" or "Response Choices").
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<string> ) )]
    public IndexableOption<string> Text { get; set; }

    /// <summary>
    /// Color of label.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> Color { get; set; }

    /// <summary>
    /// Font of the label.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont Font { get; set; } = new ChartFont { Weight = "bold" };

    /// <summary>
    /// Padding to apply around scale labels. Only top, bottom and y are implemented.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Padding { get; set; }
}