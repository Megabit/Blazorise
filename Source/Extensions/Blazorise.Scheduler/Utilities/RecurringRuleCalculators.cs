#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.Scheduler.Extensions;
#endregion

namespace Blazorise.Scheduler.Utilities;

public static class RecurringRuleCalculators
{
    public static IEnumerable<DateTime> GetDailyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, SchedulerRecurrenceRule rule )
    {
        if ( rule.Interval < 1 || rule.Interval > 99 )
            throw new ArgumentOutOfRangeException( nameof( rule.Interval ), "Interval must be between 1 and 99." );

        // Calculate how many intervals have passed before currentWeekStart
        double totalDaysSinceStart = ( viewStart - itemStart ).TotalDays;
        int intervalsBeforeWeek = Math.Max( 0, (int)Math.Floor( totalDaysSinceStart / rule.Interval ) );

        // Apply count offset if needed
        int remainingCount = rule.Count.HasValue ? Math.Max( 0, rule.Count.Value - intervalsBeforeWeek ) : int.MaxValue;

        // Determine first valid occurrence on or after currentWeekStart
        int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( totalDaysSinceStart / rule.Interval ) );
        DateTime occurrence = itemStart.AddDays( intervalsPassed * rule.Interval );

        int yielded = 0;

        while ( occurrence <= viewEnd && yielded < remainingCount )
        {
            if ( rule.EndDate.HasValue && occurrence >= rule.EndDate.Value )
                yield break;

            yield return occurrence;
            yielded++;

            occurrence = occurrence.AddDays( rule.Interval );
        }
    }

    public static IEnumerable<DateTime> GetWeeklyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, DayOfWeek firstDayOfWeek, SchedulerRecurrenceRule rule )
    {
        if ( rule.Interval < 1 )
            throw new ArgumentOutOfRangeException( nameof( rule.Interval ), "Interval must be 1 or more." );

        var daysOfWeek = ( rule.ByDay is null || rule.ByDay.Count == 0 )
            ? new List<DayOfWeek> { itemStart.DayOfWeek }
            : rule.ByDay;

        DateTime startWeek = itemStart.StartOfWeek( firstDayOfWeek );
        DateTime viewWeekStart = viewStart.StartOfWeek( firstDayOfWeek );
        int weeksBetween = (int)Math.Floor( ( viewWeekStart - startWeek ).TotalDays / 7.0 );
        int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( weeksBetween / (double)rule.Interval ) );

        // Calculate occurrences from beginning to determine if we've hit Count
        int occurrencesBeforeView = 0;

        if ( rule.Count.HasValue )
        {
            // Only calculate previous occurrences if there's a Count constraint
            for ( int i = 0; i < intervalsPassed; i++ )
            {
                DateTime recurrenceWeekStart = startWeek.AddDays( i * rule.Interval * 7 );

                foreach ( var day in daysOfWeek )
                {
                    DateTime occurrence = GetDayInWeek( recurrenceWeekStart, day, itemStart.TimeOfDay );
                    if ( occurrence >= itemStart && occurrence < viewStart )
                        occurrencesBeforeView++;
                }

                // If we've already exceeded Count before view window, exit early
                if ( rule.Count.HasValue && occurrencesBeforeView >= rule.Count.Value )
                    yield break;
            }
        }

        // Define a sensible maximum to avoid infinite loops
        int maxIterations = 99;
        int occurrencesInView = 0;
        int remainingOccurrences = rule.Count.HasValue
            ? rule.Count.Value - occurrencesBeforeView
            : int.MaxValue;

        for ( int i = intervalsPassed; i < intervalsPassed + maxIterations; i++ )
        {
            DateTime recurrenceWeekStart = startWeek.AddDays( i * rule.Interval * 7 );

            if ( recurrenceWeekStart > viewEnd )
                yield break;

            foreach ( var day in daysOfWeek.OrderBy( d => d ) )
            {
                DateTime occurrence = GetDayInWeek( recurrenceWeekStart, day, itemStart.TimeOfDay );

                if ( occurrence < itemStart )
                    continue;

                if ( occurrence < viewStart || occurrence > viewEnd )
                    continue;

                if ( rule.EndDate.HasValue && occurrence > rule.EndDate.Value )
                    yield break;

                yield return occurrence;
                occurrencesInView++;

                if ( occurrencesInView >= remainingOccurrences )
                    yield break;
            }
        }

        static DateTime GetDayInWeek( DateTime weekStart, DayOfWeek targetDay, TimeSpan time )
        {
            int offset = ( (int)targetDay - (int)weekStart.DayOfWeek + 7 ) % 7;
            return weekStart.Date.AddDays( offset ).Add( time );
        }
    }

    public static IEnumerable<DateTime> GetMonthlyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, DayOfWeek firstDayOfWeek, SchedulerRecurrenceRule rule )
    {
        if ( rule.Interval <= 0 )
            yield break;

        DateTime firstOccurrence;
        int occurrenceCount = 0;

        if ( rule.ByMonthDay?.Any() == true )
        {
            // Use the first defined day to anchor the month logic, like before
            var firstDay = rule.ByMonthDay.First();
            int firstDayResolved = firstDay > 0
                ? firstDay
                : DateTime.DaysInMonth( itemStart.Year, itemStart.Month ) + firstDay + 1;

            if ( firstDayResolved < 1 || firstDayResolved > DateTime.DaysInMonth( itemStart.Year, itemStart.Month ) )
                yield break;

            firstOccurrence = new DateTime( itemStart.Year, itemStart.Month, firstDayResolved, itemStart.Hour, itemStart.Minute, itemStart.Second );

            if ( firstOccurrence < itemStart )
                firstOccurrence = firstOccurrence.AddMonths( 1 );

            int monthsBetween = ( ( viewStart.Year - firstOccurrence.Year ) * 12 ) + ( viewStart.Month - firstOccurrence.Month );
            int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( monthsBetween / (double)rule.Interval ) );

            firstOccurrence = firstOccurrence.AddMonths( intervalsPassed * rule.Interval );
            occurrenceCount = intervalsPassed;

            while ( firstOccurrence <= viewEnd &&
                    ( !rule.Count.HasValue || occurrenceCount < rule.Count ) &&
                    ( !rule.EndDate.HasValue || firstOccurrence <= rule.EndDate ) )
            {
                int daysInMonth = DateTime.DaysInMonth( firstOccurrence.Year, firstOccurrence.Month );

                foreach ( var day in rule.ByMonthDay )
                {
                    int actualDay = day > 0 ? day : daysInMonth + day + 1;

                    if ( actualDay >= 1 && actualDay <= daysInMonth )
                    {
                        var occurrence = new DateTime( firstOccurrence.Year, firstOccurrence.Month, actualDay, itemStart.Hour, itemStart.Minute, itemStart.Second );

                        if ( occurrence >= itemStart && occurrence >= viewStart && occurrence <= viewEnd &&
                             ( !rule.EndDate.HasValue || occurrence <= rule.EndDate ) )
                        {
                            yield return occurrence;

                            occurrenceCount++;
                            if ( rule.Count.HasValue && occurrenceCount >= rule.Count )
                                yield break;
                        }
                    }
                }

                firstOccurrence = firstOccurrence.AddMonths( rule.Interval );
            }
        }
        else if ( rule.ByWeek.HasValue && rule.ByWeekDay.HasValue )
        {
            // Handle month-week-day based recurrences
            DateTime referenceDate = new DateTime( itemStart.Year, itemStart.Month, 1, itemStart.Hour, itemStart.Minute, itemStart.Second );

            int monthsBetween = ( ( viewStart.Year - referenceDate.Year ) * 12 ) + ( viewStart.Month - referenceDate.Month );
            int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( monthsBetween / (double)rule.Interval ) );

            referenceDate = referenceDate.AddMonths( intervalsPassed * rule.Interval );

            occurrenceCount = intervalsPassed;

            while ( referenceDate <= viewEnd && ( !rule.Count.HasValue || occurrenceCount < rule.Count ) && ( !rule.EndDate.HasValue || referenceDate <= rule.EndDate ) )
            {
                DateTime calculatedDate = GetNthWeekdayOfMonth( referenceDate.Year, referenceDate.Month, rule.ByWeek.Value, rule.ByWeekDay.Value )
                    .AddHours( itemStart.Hour ).AddMinutes( itemStart.Minute ).AddSeconds( itemStart.Second );

                if ( calculatedDate >= viewStart && calculatedDate <= viewEnd && calculatedDate >= itemStart && ( !rule.EndDate.HasValue || calculatedDate <= rule.EndDate ) )
                {
                    yield return calculatedDate;
                    occurrenceCount++;

                    if ( rule.Count.HasValue && occurrenceCount >= rule.Count )
                        yield break;
                }

                referenceDate = referenceDate.AddMonths( rule.Interval );
            }
        }
        else if ( rule.ByDay?.Any() == true )
        {
            firstOccurrence = new DateTime( itemStart.Year, itemStart.Month, itemStart.Day, itemStart.Hour, itemStart.Minute, itemStart.Second );

            if ( firstOccurrence < itemStart )
                firstOccurrence = firstOccurrence.AddMonths( 1 );

            int monthsBetween = ( ( viewStart.Year - firstOccurrence.Year ) * 12 ) + ( viewStart.Month - firstOccurrence.Month );
            int intervalsPassed = Math.Max( 0, (int)Math.Ceiling( monthsBetween / (double)rule.Interval ) );

            firstOccurrence = firstOccurrence.AddMonths( intervalsPassed * rule.Interval );
            occurrenceCount = intervalsPassed;

            while ( firstOccurrence <= viewEnd &&
                    ( !rule.Count.HasValue || occurrenceCount < rule.Count ) &&
                    ( !rule.EndDate.HasValue || firstOccurrence <= rule.EndDate ) )
            {
                int daysInMonth = DateTime.DaysInMonth( firstOccurrence.Year, firstOccurrence.Month );

                for ( int day = 1; day <= daysInMonth; day++ )
                {
                    var occurrence = new DateTime( firstOccurrence.Year, firstOccurrence.Month, day, itemStart.Hour, itemStart.Minute, itemStart.Second );

                    if ( rule.ByDay.Contains( occurrence.DayOfWeek ) &&
                         occurrence >= itemStart && occurrence >= viewStart && occurrence <= viewEnd &&
                         ( !rule.EndDate.HasValue || occurrence <= rule.EndDate ) )
                    {
                        yield return occurrence;

                        occurrenceCount++;
                        if ( rule.Count.HasValue && occurrenceCount >= rule.Count )
                            yield break;
                    }
                }

                firstOccurrence = firstOccurrence.AddMonths( rule.Interval );
            }
        }
    }

    // Helper to calculate the nth weekday of a month
    private static DateTime GetNthWeekdayOfMonth( int year, int month, SchedulerWeek monthWeek, DayOfWeek weekday )
    {
        DateTime firstOfMonth = new DateTime( year, month, 1 );

        int daysOffset = ( (int)weekday - (int)firstOfMonth.DayOfWeek + 7 ) % 7;
        DateTime firstWeekday = firstOfMonth.AddDays( daysOffset );

        int weekNumber = monthWeek switch
        {
            SchedulerWeek.First => 0,
            SchedulerWeek.Second => 1,
            SchedulerWeek.Third => 2,
            SchedulerWeek.Fourth => 3,
            SchedulerWeek.Last => ( firstWeekday.AddDays( 28 ).Month == month ) ? 4 : 3,
            _ => throw new ArgumentOutOfRangeException( nameof( monthWeek ), monthWeek, null )
        };

        DateTime result = firstWeekday.AddDays( weekNumber * 7 );

        if ( monthWeek == SchedulerWeek.Last && result.AddDays( 7 ).Month == month )
        {
            result = result.AddDays( 7 );
        }

        return result;
    }

    public static IEnumerable<DateTime> GetYearlyRecurringDates( DateTime itemStart, DateTime viewStart, DateTime viewEnd, DayOfWeek firstDayOfWeek, SchedulerRecurrenceRule rule )
    {
        if ( rule.Interval <= 0 )
            yield break;

        DateTime occurrence;
        int occurrencesCount = 0;

        int startYear = Math.Max( itemStart.Year, viewStart.Year );
        int endYear = viewEnd.Year;

        for ( int year = startYear; year <= endYear; year += rule.Interval )
        {
            if ( rule.ByMonth.HasValue )
            {
                int month = (int)rule.ByMonth.Value;

                if ( rule.ByWeek.HasValue && rule.ByWeekDay.HasValue )
                {
                    occurrence = GetNthWeekdayOfMonth( year, month, rule.ByWeek.Value, rule.ByWeekDay.Value )
                        .AddHours( itemStart.Hour )
                        .AddMinutes( itemStart.Minute )
                        .AddSeconds( itemStart.Second );
                }
                else
                {
                    // Default to the start date's day if no ByWeek/ByWeekDay defined
                    int day = Math.Min( itemStart.Day, DateTime.DaysInMonth( year, month ) );
                    occurrence = new DateTime( year, month, day, itemStart.Hour, itemStart.Minute, itemStart.Second );
                }

                if ( occurrence >= itemStart && occurrence >= viewStart && occurrence <= viewEnd &&
                    ( !rule.EndDate.HasValue || occurrence <= rule.EndDate.Value ) )
                {
                    yield return occurrence;
                    occurrencesCount++;

                    if ( rule.Count.HasValue && occurrencesCount >= rule.Count )
                        yield break;
                }
            }
            else
            {
                // If no ByMonth defined, default to itemStart month
                int month = itemStart.Month;
                int day = Math.Min( itemStart.Day, DateTime.DaysInMonth( year, month ) );

                occurrence = new DateTime( year, month, day, itemStart.Hour, itemStart.Minute, itemStart.Second );

                if ( occurrence >= itemStart && occurrence >= viewStart && occurrence <= viewEnd &&
                    ( !rule.EndDate.HasValue || occurrence <= rule.EndDate.Value ) )
                {
                    yield return occurrence;
                    occurrencesCount++;

                    if ( rule.Count.HasValue && occurrencesCount >= rule.Count )
                        yield break;
                }
            }
        }
    }
}
