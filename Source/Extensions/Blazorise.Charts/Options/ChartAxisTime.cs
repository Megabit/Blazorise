#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Common time options to all axes.
/// </summary>
public class ChartAxisTime
{
    /// <summary>
    /// Sets how different time units are displayed.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxisTimeDisplayFormat DisplayFormats { get; set; }

    /// <summary>
    /// If boolean and true and the unit is set to 'week', then the first day of the week will be Monday. Otherwise, it will be Sunday. If number, the index of the first day of the week (0 - Sunday, 6 - Saturday)
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object IsoWeekday { get; set; }

    /// <summary>
    /// Custom parser for dates.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Parser { get; set; }

    /// <summary>
    /// If defined, dates will be rounded to the start of this unit. See <see href="https://www.chartjs.org/docs/latest/axes/cartesian/time.html#time-units">Time Units</see> section below for details.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Round { get; set; }

    /// <summary>
    /// If defined, will force the unit to be a certain type. See <see href="https://www.chartjs.org/docs/latest/axes/cartesian/time.html#time-units">Time Units</see> section below for details.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Unit { get; set; }

    /// <summary>
    /// The number of units between grid lines.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? StepSize { get; set; }

    /// <summary>
    /// The minimum display format to be used for a time unit.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string MinUnit { get; set; }
}