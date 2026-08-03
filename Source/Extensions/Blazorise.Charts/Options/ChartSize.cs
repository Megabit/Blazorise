#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Supports chart size behavior in chart components.
/// </summary>
public class ChartSize
{
    /// <summary>
    /// Rendered width of the element.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Width { get; set; }

    /// <summary>
    /// Rendered height of the element.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Height { get; set; }
}