#region Using directives
using System;
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
}