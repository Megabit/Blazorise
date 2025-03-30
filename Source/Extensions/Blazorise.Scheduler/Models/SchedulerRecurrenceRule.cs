#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Defines the recurrence pattern for a scheduler event.
/// </summary>
public class SchedulerRecurrenceRule
{
    #region Methods

    /// <summary>
    /// Generates a string representation of a recurrence rule (RRULE) based on specified pattern and parameters.
    /// </summary>
    /// <returns>A formatted string that represents the recurrence rule.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid day of the week is encountered in the ByDay collection.</exception>
    public string ToRRuleString()
    {
        var components = new List<string>
        {
            $"FREQ={Pattern.ToString().ToUpperInvariant()}",
        };

        if ( Interval > 1 )
            components.Add( $"INTERVAL={Interval}" );

        if ( ByDay != null && ByDay.Any() )
        {
            var byDay = string.Join( ",", ByDay.Select( d => d switch
            {
                DayOfWeek.Monday => "MO",
                DayOfWeek.Tuesday => "TU",
                DayOfWeek.Wednesday => "WE",
                DayOfWeek.Thursday => "TH",
                DayOfWeek.Friday => "FR",
                DayOfWeek.Saturday => "SA",
                DayOfWeek.Sunday => "SU",
                _ => throw new ArgumentOutOfRangeException()
            } ) );
            components.Add( $"BYDAY={byDay}" );
        }

        if ( Count.HasValue )
            components.Add( $"COUNT={Count.Value}" );
        else if ( EndDate.HasValue )
            components.Add( $"UNTIL={EndDate.Value.ToUniversalTime():yyyyMMdd'T'HHmmss'Z'}" );

        return string.Join( ";", components );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Represents the recurrence pattern for a scheduler event.
    /// </summary>
    public SchedulerRecurrencePattern Pattern { get; set; }

    /// <summary>
    /// Represents the time interval, measured in integer units.
    /// </summary>
    public int Interval { get; set; } = 1;

    /// <summary>
    /// A list of days of the week. It allows for specifying multiple days for scheduling or recurring events.
    /// </summary>
    public List<DayOfWeek> ByDay { get; set; }

    /// <summary>
    /// Represents the optional end date of an event or period. It can hold a null value if no end date is specified.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Defines how many total occurrences of the event should happen.
    /// </summary>
    public int? Count { get; set; }

    #endregion
}