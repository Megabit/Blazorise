#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Display formats for time options.
/// </summary>
public class ChartAxisTimeDisplayFormat
{
    /// <summary>
    /// Display format for millisecond ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Millisecond { get; set; }

    /// <summary>
    /// Display format for second ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Second { get; set; }

    /// <summary>
    /// Display format for minute ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Minute { get; set; }

    /// <summary>
    /// Display format for hourly ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Hour { get; set; }

    /// <summary>
    /// Display format for daily ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Day { get; set; }

    /// <summary>
    /// Display format for weekly ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Week { get; set; }

    /// <summary>
    /// Display format for monthly ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Month { get; set; }

    /// <summary>
    /// Display format for quarterly ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Quarter { get; set; }

    /// <summary>
    /// Display format for yearly ticks.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Year { get; set; }
}