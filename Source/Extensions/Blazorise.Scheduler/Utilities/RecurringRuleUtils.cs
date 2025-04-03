#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Scheduler.Extensions;
#endregion

namespace Blazorise.Scheduler.Utilities;

internal static class RecurringRuleUtils
{
    public static IEnumerable<DateTime> GetDailyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, int interval, DateTime? endDate, int? count )
    {
        if ( interval < 1 || interval > 99 )
            throw new ArgumentOutOfRangeException( nameof( interval ), "Interval must be between 1 and 99." );

        // Calculate how many intervals have passed before currentWeekStart
        double totalDaysSinceStart = ( viewStart - itemStart ).TotalDays;
        int intervalsBeforeWeek = Math.Max( 0, (int)Math.Floor( totalDaysSinceStart / interval ) );

        // Apply count offset if needed
        int remainingCount = count.HasValue ? Math.Max( 0, count.Value - intervalsBeforeWeek ) : int.MaxValue;

        // Determine first valid occurrence on or after currentWeekStart
        int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( totalDaysSinceStart / interval ) );
        DateTime occurrence = itemStart.AddDays( intervalsPassed * interval );

        int yielded = 0;

        while ( occurrence <= viewEnd && yielded < remainingCount )
        {
            if ( endDate.HasValue && occurrence > endDate.Value )
                yield break;

            yield return occurrence;
            yielded++;

            occurrence = occurrence.AddDays( interval );
        }
    }

    public static IEnumerable<DateTime> GetWeeklyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, DateTime? endDate, int interval, List<DayOfWeek> byDay, int? count, DayOfWeek firstDayOfWeek )
    {
        if ( interval < 1 )
            throw new ArgumentOutOfRangeException( nameof( interval ), "Interval must be 1 or more." );

        if ( byDay == null || byDay.Count == 0 )
            throw new ArgumentException( "At least one day must be specified in byDay.", nameof( byDay ) );

        DateTime startWeek = itemStart.StartOfWeek( firstDayOfWeek );
        DateTime viewWeekStart = viewStart.StartOfWeek( firstDayOfWeek );

        // How many full weeks between start and current view
        int weeksBetween = (int)Math.Floor( ( viewWeekStart - startWeek ).TotalDays / 7.0 );
        int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( weeksBetween / (double)interval ) );

        // Calculate the next valid recurrence week
        DateTime recurrenceWeekStart = startWeek.AddDays( intervalsPassed * interval * 7 );

        // Check if recurrence is beyond count
        if ( count.HasValue && intervalsPassed >= count.Value )
            yield break;

        // Don't go beyond the view or endDate
        if ( recurrenceWeekStart > viewEnd )
            yield break;

        foreach ( var day in byDay.OrderBy( d => d ) )
        {
            DateTime occurrence = GetDayInWeek( recurrenceWeekStart, day, itemStart.TimeOfDay );

            if ( occurrence < itemStart )
                continue;

            if ( occurrence < viewStart || occurrence > viewEnd )
                continue;

            if ( endDate.HasValue && occurrence > endDate.Value )
                continue;

            yield return occurrence;
        }

        static DateTime GetDayInWeek( DateTime weekStart, DayOfWeek targetDay, TimeSpan time )
        {
            int offset = ( (int)targetDay - (int)weekStart.DayOfWeek + 7 ) % 7;
            return weekStart.Date.AddDays( offset ).Add( time );
        }
    }
}
