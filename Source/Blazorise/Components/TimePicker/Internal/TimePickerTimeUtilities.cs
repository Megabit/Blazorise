#region Using directives
using System;
using System.Globalization;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Provides time conversion, constraint, and formatting helpers used by a <see cref="TimePicker{TValue}"/>.
/// </summary>
internal static class TimePickerTimeUtilities
{
    #region Methods

    /// <summary>
    /// Attempts to convert a picker value to a time of day.
    /// </summary>
    /// <param name="value">Picker value to convert.</param>
    /// <param name="result">Converted time of day.</param>
    /// <returns><see langword="true"/> when the value has a supported type.</returns>
    public static bool TryGetTime( object value, out TimeSpan result )
    {
        switch ( value )
        {
            case TimeSpan timeSpan:
                result = Normalize( timeSpan );
                return true;
            case TimeOnly timeOnly:
                result = timeOnly.ToTimeSpan();
                return true;
            case DateTime dateTime:
                result = dateTime.TimeOfDay;
                return true;
            default:
                result = default;
                return false;
        }
    }

    /// <summary>
    /// Creates the default time constrained to the configured range.
    /// </summary>
    /// <param name="defaultHour">Default hour component.</param>
    /// <param name="defaultMinute">Default minute component.</param>
    /// <param name="min">Optional minimum time.</param>
    /// <param name="max">Optional maximum time.</param>
    /// <returns>The constrained default time.</returns>
    public static TimeSpan GetDefault( int defaultHour, int defaultMinute, TimeSpan? min, TimeSpan? max )
    {
        TimeSpan result = new(
            Math.Clamp( defaultHour, 0, 23 ),
            Math.Clamp( defaultMinute, 0, 59 ),
            0 );

        return Clamp( result, min, max );
    }

    /// <summary>
    /// Constrains a time to the configured range.
    /// </summary>
    /// <param name="time">Time to constrain.</param>
    /// <param name="min">Optional minimum time.</param>
    /// <param name="max">Optional maximum time.</param>
    /// <returns>The normalized and constrained time.</returns>
    public static TimeSpan Clamp( TimeSpan time, TimeSpan? min, TimeSpan? max )
    {
        time = Normalize( time );

        if ( min.HasValue && time < Normalize( min.Value ) )
        {
            time = Normalize( min.Value );
        }

        if ( max.HasValue && time > Normalize( max.Value ) )
        {
            time = Normalize( max.Value );
        }

        return time;
    }

    /// <summary>
    /// Normalizes a time to one 24-hour day.
    /// </summary>
    /// <param name="time">Time to normalize.</param>
    /// <returns>A time between midnight and the end of the day.</returns>
    public static TimeSpan Normalize( TimeSpan time )
    {
        long ticks = time.Ticks % TimeSpan.TicksPerDay;

        if ( ticks < 0 )
        {
            ticks += TimeSpan.TicksPerDay;
        }

        return TimeSpan.FromTicks( ticks );
    }

    /// <summary>
    /// Formats a time for display using the current culture.
    /// </summary>
    /// <param name="time">Time to format.</param>
    /// <param name="format">Display format.</param>
    /// <returns>The formatted time.</returns>
    public static string FormatDisplay( TimeSpan time, string format )
        => DateTime.Today.Add( Normalize( time ) ).ToString( format, CultureInfo.CurrentCulture );

    /// <summary>
    /// Formats a time using the internal picker representation.
    /// </summary>
    /// <param name="time">Time to format.</param>
    /// <returns>The internal time representation.</returns>
    public static string FormatInternal( TimeSpan time )
        => Normalize( time ).ToString( Parsers.InternalTimeFormat.ToLowerInvariant(), CultureInfo.InvariantCulture );

    /// <summary>
    /// Formats a time for a native time input.
    /// </summary>
    /// <param name="time">Time to format.</param>
    /// <param name="includeSeconds">Whether the output should include seconds.</param>
    /// <returns>The native time input representation.</returns>
    public static string FormatNative( TimeSpan time, bool includeSeconds )
        => Normalize( time ).ToString( includeSeconds ? @"hh\:mm\:ss" : @"hh\:mm", CultureInfo.InvariantCulture );

    #endregion
}