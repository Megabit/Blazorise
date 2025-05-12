#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Scheduler.Extensions;

/// <summary>
/// Extension methods for the <see cref="DateTime"/> class.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the first day of the week for the given date.
    /// </summary>
    /// <param name="dt">The date to get the first day of the week for.</param>
    /// <param name="startOfWeek">The day of the week that should be considered the first day of the week.</param>
    /// <returns>The first day of the week for the given date.</returns>
    public static DateTime StartOfWeek( this DateTime dt, DayOfWeek startOfWeek )
    {
        int diff = ( 7 + ( dt.DayOfWeek - startOfWeek ) ) % 7;
        return dt.AddDays( -1 * diff );
    }

    /// <summary>
    /// Returns the last day of the week for the given date.
    /// </summary>
    /// <param name="dt">The date to get the last day of the week for.</param>
    /// <param name="startOfWeek">The day of the week that should be considered the first day of the week.</param>
    /// <returns>The last day of the week for the given date.</returns>
    public static DateTime EndOfWeek( this DateTime dt, DayOfWeek startOfWeek )
    {
        return StartOfWeek( dt, startOfWeek ).AddDays( 6 );
    }

    /// <summary>
    /// Returns the first day of the week for the given date.
    /// </summary>
    /// <param name="dt">The date to get the first day of the week for.</param>
    /// <param name="startOfWeek">The day of the week that should be considered the first day of the week.</param>
    /// <returns>The first day of the week for the given date.</returns>
    public static DateOnly StartOfWeek( this DateOnly dt, DayOfWeek startOfWeek )
    {
        int diff = ( 7 + ( dt.DayOfWeek - startOfWeek ) ) % 7;
        return dt.AddDays( -1 * diff );
    }

    /// <summary>
    /// Returns the first day of the previous week for the given date.
    /// </summary>
    /// <param name="dt">The date to get the first day of the week for.</param>
    /// <param name="startOfWeek">The day of the week that should be considered the first day of the week.</param>
    /// <returns>The first day of the previous week for the given date.</returns>
    public static DateOnly StartOfPreviousWeek( this DateOnly dt, DayOfWeek startOfWeek )
    {
        return StartOfWeek( dt, startOfWeek ).AddDays( -7 );
    }

    /// <summary>
    /// Returns the first day of the next week for the given date.
    /// </summary>
    /// <param name="dt">The date to get the first day of the week for.</param>
    /// <param name="startOfWeek">The day of the week that should be considered the first day of the week.</param>
    /// <returns>The first day of the week for the given date.</returns>
    public static DateOnly StartOfNextWeek( this DateOnly dt, DayOfWeek startOfWeek )
    {
        return StartOfWeek( dt, startOfWeek ).AddDays( 7 );
    }

    /// <summary>
    /// Returns the first day of the next week for the given date.
    /// </summary>
    /// <param name="dt">The date to get the first day of the week for.</param>
    /// <param name="startOfWeek">The day of the week that should be considered the first day of the week.</param>
    /// <returns>The first day of the week for the given date.</returns>
    public static DateTime StartOfNextWeek( this DateTime dt, DayOfWeek startOfWeek )
    {
        return StartOfWeek( dt, startOfWeek ).AddDays( 7 );
    }

    /// <summary>
    /// Calculates the first day of the month for a given date.
    /// </summary>
    /// <param name="dt">Represents a specific date from which the first day of the month will be determined.</param>
    /// <returns>Returns a new date representing the first day of the month of the provided date.</returns>
    public static DateOnly StartOfMonth( this DateOnly dt )
    {
        return new DateOnly( dt.Year, dt.Month, 1 );
    }

    /// <summary>
    /// Calculates the last day of the month for a given date.
    /// </summary>
    /// <param name="dt">Represents a specific date from which the end of the month is determined.</param>
    /// <returns>Returns a new date representing the last day of the month.</returns>
    public static DateOnly EndOfMonth( this DateOnly dt )
    {
        return new DateOnly( dt.Year, dt.Month, DateTime.DaysInMonth( dt.Year, dt.Month ) );
    }

    /// <summary>
    /// Returns the week number using a specified first day of the week.
    /// Uses a "first week has at least 4 days" rule (like ISO 8601).
    /// </summary>
    public static int GetWeekNumber( this DateOnly date, DayOfWeek firstDayOfWeek )
    {
        var dt = date.ToDateTime( TimeOnly.MinValue, DateTimeKind.Unspecified );

        // Normalize the DayOfWeek to the given firstDayOfWeek context
        var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek( dt );
        int offset = ( (int)day - (int)firstDayOfWeek + 7 ) % 7;

        // If we're in the first 3 days of the week relative to firstDayOfWeek, shift forward
        if ( offset <= 2 )
        {
            dt = dt.AddDays( 3 );
        }

        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear( dt, CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek );
    }

    /// <summary>
    /// Returns the earlier of the two specified DateTime values.
    /// </summary>
    /// <param name="first">The first DateTime value.</param>
    /// <param name="second">The second DateTime value.</param>
    /// <returns>The earlier (minimum) of the two DateTime values.</returns>
    public static DateTime Min( this DateTime first, DateTime second )
    {
        return first < second ? first : second;
    }

    /// <summary>
    /// Returns the later of the two specified DateTime values.
    /// </summary>
    /// <param name="first">The first DateTime value.</param>
    /// <param name="second">The second DateTime value.</param>
    /// <returns>The later (maximum) of the two DateTime values.</returns>
    public static DateTime Max( this DateTime first, DateTime second )
    {
        return first > second ? first : second;
    }
}