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
        else if ( ( Pattern == SchedulerRecurrencePattern.Monthly || Pattern == SchedulerRecurrencePattern.Yearly ) && ByWeek.HasValue && ByWeekDay.HasValue )
        {
            var ordinal = ( (int)ByWeek.Value ).ToString();
            var day = DayToRfc5545( ByWeekDay.Value );
            components.Add( $"BYDAY={ordinal}{day}" );
        }

        // BYMONTHDAY rule: monthly by date(s) of month
        if ( Pattern == SchedulerRecurrencePattern.Monthly && ByMonthDay?.Any() == true )
        {
            var byMonthDay = string.Join( ",", ByMonthDay );
            components.Add( $"BYMONTHDAY={byMonthDay}" );
        }

        // BYMONTH rule: yearly by month
        if ( Pattern == SchedulerRecurrencePattern.Yearly && ByMonth.HasValue )
        {
            // Use the integer value directly since we've now assigned explicit values
            int monthNumber = (int)ByMonth.Value;
            components.Add( $"BYMONTH={monthNumber}" );
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

    /// <summary>
    /// A list of days of the month. It allows for specifying multiple days for scheduling or recurring events.
    /// </summary>
    public List<int> ByMonthDay { get; set; }

    #endregion

    #region Monthly Rules

    /// <summary>
    /// Defines the week of the month when an event should occur.
    /// </summary>
    public SchedulerWeek? ByWeek { get; set; }

    /// <summary>
    /// Defines the day of the week when an event should occur.
    /// </summary>
    public DayOfWeek? ByWeekDay { get; set; }

    #endregion

    #region Yearly Rules

    /// <summary>
    /// Defines the month of the year when an event should occur.
    /// </summary>
    public SchedulerMonth? ByMonth { get; set; }

    #endregion

    #endregion
}