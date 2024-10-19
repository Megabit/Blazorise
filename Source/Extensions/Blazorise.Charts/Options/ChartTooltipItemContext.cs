#region Using directives
using System.Text.Json.Serialization;
using Lambda2Js;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// The tooltip items passed to the tooltip callbacks implement the following interface.
/// </summary>
public class ChartTooltipItemContext
{
    /// <summary>
    /// Label for the tooltip.
    /// </summary>
    [JsonPropertyName( "index" )]
    [JavascriptMember( MemberName = "index" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Label { get; set; }

    /// <summary>
    /// Parsed data values for the given `dataIndex` and `datasetIndex`.
    /// </summary>
    [JsonPropertyName( "parsed" )]
    [JavascriptMember( MemberName = "parsed" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Parsed { get; set; }

    /// <summary>
    /// Raw data values for the given `dataIndex` and `datasetIndex`.
    /// </summary>
    [JsonPropertyName( "raw" )]
    [JavascriptMember( MemberName = "raw" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Raw { get; set; }

    /// <summary>
    /// Formatted value for the tooltip.
    /// </summary>
    [JsonPropertyName( "formattedValue" )]
    [JavascriptMember( MemberName = "formattedValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string FormattedValue { get; set; }

    /// <summary>
    /// The dataset the item comes from.
    /// </summary>
    [JsonPropertyName( "dataset" )]
    [JavascriptMember( MemberName = "dataset" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Dataset { get; set; }

    /// <summary>
    /// Index of the dataset the item comes from.
    /// </summary>
    [JsonPropertyName( "datasetIndex" )]
    [JavascriptMember( MemberName = "datasetIndex" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int DatasetIndex { get; set; }

    /// <summary>
    /// Index of this data item in the dataset.
    /// </summary>
    [JsonPropertyName( "dataIndex" )]
    [JavascriptMember( MemberName = "dataIndex" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int DataIndex { get; set; }

    /// <summary>
    /// The chart element (point, arc, bar, etc.) for this tooltip item.
    /// </summary>
    [JsonPropertyName( "element" )]
    [JavascriptMember( MemberName = "element" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Element { get; set; }
}