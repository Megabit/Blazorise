#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Chart scales configuration.
/// </summary>
public class ChartScales
{
    /// <summary>
    /// Configuration for the x-axis.
    /// </summary>
    [JsonPropertyName( "x" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxis X { get; set; }

    /// <summary>
    /// Configuration for the y-axis.
    /// </summary>
    [JsonPropertyName( "y" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxis Y { get; set; }
}