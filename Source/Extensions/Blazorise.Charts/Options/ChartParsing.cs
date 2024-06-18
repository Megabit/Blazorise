#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Configuration for parsing chart data.
/// </summary>
public class ChartParsing
{
    /// <summary>
    /// Key for the x-axis values in the data.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string XAxisKey { get; set; }

    /// <summary>
    /// Key for the y-axis values in the data.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string YAxisKey { get; set; }
}