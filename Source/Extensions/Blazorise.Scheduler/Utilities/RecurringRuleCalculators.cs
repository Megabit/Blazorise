#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Scheduler.Extensions;
#endregion

namespace Blazorise.Scheduler.Utilities;

internal static class RecurringRuleCalculators
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

    public static IEnumerable<DateTime> GetMonthlyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, DateTime? endDate, int interval, int? byMonthDay, SchedulerMonthWeekPosition? byMonthWeek, DayOfWeek? byMonthWeekDay, int? count, DayOfWeek firstDayOfWeek )
    {
        if ( interval <= 0 )
            yield break;

        DateTime firstOccurrence;
        int occurrenceCount = 0;

        if ( byMonthDay.HasValue )
        {
            // First potential occurrence
            firstOccurrence = new DateTime( itemStart.Year, itemStart.Month, byMonthDay.Value, itemStart.Hour, itemStart.Minute, itemStart.Second );

            if ( firstOccurrence < itemStart )
                firstOccurrence = firstOccurrence.AddMonths( 1 );

            int monthsBetween = ( ( viewStart.Year - firstOccurrence.Year ) * 12 ) + ( viewStart.Month - firstOccurrence.Month );
            int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( monthsBetween / (double)interval ) );

            firstOccurrence = firstOccurrence.AddMonths( intervalsPassed * interval );

            occurrenceCount = intervalsPassed;

            while ( firstOccurrence <= viewEnd && ( !count.HasValue || occurrenceCount < count ) && ( !endDate.HasValue || firstOccurrence <= endDate ) )
            {
                if ( firstOccurrence >= viewStart )
                {
                    yield return firstOccurrence;
                    occurrenceCount++;

                    if ( count.HasValue && occurrenceCount >= count )
                        yield break;
                }

                firstOccurrence = firstOccurrence.AddMonths( interval );
            }
        }
        else if ( byMonthWeek.HasValue && byMonthWeekDay.HasValue )
        {
            // Handle month-week-day based recurrences
            DateTime referenceDate = new DateTime( itemStart.Year, itemStart.Month, 1, itemStart.Hour, itemStart.Minute, itemStart.Second );

            int monthsBetween = ( ( viewStart.Year - referenceDate.Year ) * 12 ) + ( viewStart.Month - referenceDate.Month );
            int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( monthsBetween / (double)interval ) );

            referenceDate = referenceDate.AddMonths( intervalsPassed * interval );

            occurrenceCount = intervalsPassed;

            while ( referenceDate <= viewEnd && ( !count.HasValue || occurrenceCount < count ) && ( !endDate.HasValue || referenceDate <= endDate ) )
            {
                DateTime calculatedDate = GetNthWeekdayOfMonth( referenceDate.Year, referenceDate.Month, byMonthWeek.Value, byMonthWeekDay.Value )
                    .AddHours( itemStart.Hour ).AddMinutes( itemStart.Minute ).AddSeconds( itemStart.Second );

                if ( calculatedDate >= viewStart && calculatedDate <= viewEnd && calculatedDate >= itemStart && ( !endDate.HasValue || calculatedDate <= endDate ) )
                {
                    yield return calculatedDate;
                    occurrenceCount++;

                    if ( count.HasValue && occurrenceCount >= count )
                        yield break;
                }

                referenceDate = referenceDate.AddMonths( interval );
            }
        }
    }

    // Helper to calculate the nth weekday of a month
    private static DateTime GetNthWeekdayOfMonth( int year, int month, SchedulerMonthWeekPosition monthWeek, DayOfWeek weekday )
    {
        DateTime firstOfMonth = new DateTime( year, month, 1 );

        int daysOffset = ( (int)weekday - (int)firstOfMonth.DayOfWeek + 7 ) % 7;
        DateTime firstWeekday = firstOfMonth.AddDays( daysOffset );

        int weekNumber = monthWeek switch
        {
            SchedulerMonthWeekPosition.First => 0,
            SchedulerMonthWeekPosition.Second => 1,
            SchedulerMonthWeekPosition.Third => 2,
            SchedulerMonthWeekPosition.Fourth => 3,
            SchedulerMonthWeekPosition.Last => ( firstWeekday.AddDays( 28 ).Month == month ) ? 4 : 3,
            _ => throw new ArgumentOutOfRangeException( nameof( monthWeek ), monthWeek, null )
        };

        DateTime result = firstWeekday.AddDays( weekNumber * 7 );

        if ( monthWeek == SchedulerMonthWeekPosition.Last && result.AddDays( 7 ).Month == month )
        {
            result = result.AddDays( 7 );
        }

        return result;
    }
}
