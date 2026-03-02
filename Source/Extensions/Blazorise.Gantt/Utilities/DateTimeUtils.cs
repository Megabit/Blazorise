#region Using directives
using System;
#endregion

namespace Blazorise.Gantt.Utilities;

/// <summary>
/// Provides helper methods for <see cref="DateTime"/> values.
/// </summary>
public static class DateTimeUtils
{
    /// <summary>
    /// Determines whether date value is not assigned.
    /// </summary>
    /// <param name="value">Date value.</param>
    /// <returns>True when value is min or max date; otherwise false.</returns>
    public static bool IsUnassigned( DateTime value )
    {
        return value == DateTime.MinValue || value == DateTime.MaxValue;
    }

    /// <summary>
    /// Gets duration in whole days from start and end date values.
    /// </summary>
    /// <param name="start">Start date.</param>
    /// <param name="end">End date.</param>
    /// <returns>Duration in days, always at least 1.</returns>
    public static int GetDurationInDays( DateTime start, DateTime end )
    {
        var totalDays = ( end - start ).TotalDays;

        if ( totalDays <= 0d )
            return 1;

        return Math.Max( 1, (int)Math.Ceiling( totalDays ) );
    }
}