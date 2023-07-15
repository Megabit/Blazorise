using System.Text.Json.Serialization;

namespace Blazorise.Charts.Trendline;

/// <summary>
/// Supplies information about a trendline.
/// </summary>
public class ChartTrendlineData
{
    /// <summary>
    /// The index of the dataset to add the trendline to. By default this is 0. If you have more than one line, increase the index by one for each dataset you have.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? DatasetIndex { get; set; } = 0;

    /// <summary>
    /// The colour of the trendline
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartColor? Color { get; set; } = ChartColor.FromRgba( 100, 0, 0, 1 );

    /// <summary>
    /// Can be "solid" or "dotted"
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string LineStyle { get; set; } = "solid";

    /// <summary>
    /// Line width.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Width { get; set; } = 2;

    /// <summary>
    /// Set to true to enable projection (optional).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Projection { get; set; }
}