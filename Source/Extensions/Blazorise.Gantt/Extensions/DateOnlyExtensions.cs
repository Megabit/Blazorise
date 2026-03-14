#region Using directives
using System;
#endregion

namespace Blazorise.Gantt.Extensions;

/// <summary>
/// Date helpers for Gantt chart range calculations.
/// </summary>
internal static class DateOnlyExtensions
{
    /// <summary>
    /// Returns start of week for the provided date.
    /// </summary>
    public static DateOnly StartOfWeek( this DateOnly date, DayOfWeek startOfWeek )
    {
        int diff = ( 7 + ( date.DayOfWeek - startOfWeek ) ) % 7;

        return date.AddDays( -diff );
    }

    /// <summary>
    /// Returns start of month for the provided date.
    /// </summary>
    public static DateOnly StartOfMonth( this DateOnly date )
    {
        return new DateOnly( date.Year, date.Month, 1 );
    }
}