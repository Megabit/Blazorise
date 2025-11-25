#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blazorise.Charts.Convertes;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Chart scales configuration.
/// </summary>
[JsonConverter( typeof( ChartScalesJsonConverter ) )]
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

    /// <summary>
    /// Configuration for custom named axes.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public Dictionary<string, ChartAxis> AdditionalAxes { get; set; }
}