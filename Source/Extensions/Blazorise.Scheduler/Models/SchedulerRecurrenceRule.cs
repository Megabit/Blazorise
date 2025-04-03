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

        if ( Count.HasValue )
            components.Add( $"COUNT={Count.Value}" );
        else if ( EndDate.HasValue )
            components.Add( $"UNTIL={EndDate.Value.ToUniversalTime():yyyyMMdd'T'HHmmss'Z'}" );

        // BYDAY rule: weekly, or monthly by weekday
        if ( Pattern == SchedulerRecurrencePattern.Weekly && ByDay?.Any() == true )
        {
            var byDay = string.Join( ",", ByDay.Select( DayToRfc5545 ) );
            components.Add( $"BYDAY={byDay}" );
        }
        else if ( Pattern == SchedulerRecurrencePattern.Monthly && ByMonthWeek.HasValue && ByMonthWeekDay.HasValue )
        {
            var ordinal = ( (int)ByMonthWeek.Value ).ToString();
            var day = DayToRfc5545( ByMonthWeekDay.Value );
            components.Add( $"BYDAY={ordinal}{day}" );
        }

        // BYMONTHDAY rule: monthly by day number
        if ( Pattern == SchedulerRecurrencePattern.Monthly && ByMonthDay.HasValue )
        {
            components.Add( $"BYMONTHDAY={ByMonthDay.Value}" );
        }

        return string.Join( ";", components );
    }

    /// <summary>
    /// Converts a .NET DayOfWeek into RFC 5545 two-letter day code.
    /// </summary>
    private static string DayToRfc5545( DayOfWeek day ) => day switch
    {
        DayOfWeek.Monday => "MO",
        DayOfWeek.Tuesday => "TU",
        DayOfWeek.Wednesday => "WE",
        DayOfWeek.Thursday => "TH",
        DayOfWeek.Friday => "FR",
        DayOfWeek.Saturday => "SA",
        DayOfWeek.Sunday => "SU",
        _ => throw new ArgumentOutOfRangeException( nameof( day ), $"Unsupported day: {day}" )
    };

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
    /// Represents the optional end date of an event or period. It can hold a null value if no end date is specified.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Defines how many total occurrences of the event should happen.
    /// </summary>
    public int? Count { get; set; }

    #region Weekly Rules

    /// <summary>
    /// A list of days of the week. It allows for specifying multiple days for scheduling or recurring events.
    /// </summary>
    public List<DayOfWeek> ByDay { get; set; }

    #endregion

    #region Monthly Rules

    public int? ByMonthDay { get; set; }

    public SchedulerMonthWeekPosition? ByMonthWeek { get; set; }

    public DayOfWeek? ByMonthWeekDay { get; set; }

    #endregion

    #endregion
}