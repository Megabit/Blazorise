#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Provides date conversion and navigation helpers used by a <see cref="DatePicker{TValue}"/>.
/// </summary>
internal static class DatePickerDateUtilities
{
    #region Methods

    /// <summary>
    /// Determines whether an enumerable contains at least one item.
    /// </summary>
    /// <param name="values">Values to inspect.</param>
    /// <returns><see langword="true"/> when at least one item is present.</returns>
    public static bool HasItems( IEnumerable values )
    {
        if ( values is null )
            return false;

        IEnumerator enumerator = values.GetEnumerator();

        try
        {
            return enumerator.MoveNext();
        }
        finally
        {
            ( enumerator as IDisposable )?.Dispose();
        }
    }

    /// <summary>
    /// Converts a picker value to its selected dates.
    /// </summary>
    /// <param name="value">Picker value to convert.</param>
    /// <param name="selectionMode">Active date selection mode.</param>
    /// <returns>The dates represented by the picker value.</returns>
    public static IReadOnlyList<DateTime> GetSelectedDates( object value, DateInputSelectionMode selectionMode )
    {
        List<DateTime> result = new();

        if ( value is null )
            return result;

        if ( selectionMode != DateInputSelectionMode.Single && value is IEnumerable values )
        {
            foreach ( object item in values )
            {
                if ( TryConvertToDateTime( item, out DateTime date ) )
                {
                    result.Add( date );
                }
            }
        }
        else if ( TryConvertToDateTime( value, out DateTime date ) )
        {
            result.Add( date );
        }

        return result;
    }

    /// <summary>
    /// Creates the initial date used when the picker does not have a value.
    /// </summary>
    /// <param name="defaultHour">Default hour component.</param>
    /// <param name="defaultMinute">Default minute component.</param>
    /// <param name="min">Optional minimum date.</param>
    /// <param name="max">Optional maximum date.</param>
    /// <returns>The initial date constrained to the configured date range.</returns>
    public static DateTime GetInitialDate( int defaultHour, int defaultMinute, DateTimeOffset? min, DateTimeOffset? max )
    {
        DateTime initial = DateTime.Today
            .AddHours( Math.Clamp( defaultHour, 0, 23 ) )
            .AddMinutes( Math.Clamp( defaultMinute, 0, 59 ) );

        if ( min.HasValue && initial.Date < min.Value.Date )
        {
            initial = min.Value.DateTime;
        }

        if ( max.HasValue && initial.Date > max.Value.Date )
        {
            initial = max.Value.DateTime;
        }

        return initial;
    }

    /// <summary>
    /// Determines whether an enumerable contains the specified calendar date.
    /// </summary>
    /// <param name="values">Values to inspect.</param>
    /// <param name="date">Calendar date to locate.</param>
    /// <returns><see langword="true"/> when the date is present.</returns>
    public static bool ContainsDate( IEnumerable values, DateTime date )
    {
        foreach ( DateTime item in EnumerateDates( values ) )
        {
            if ( item.Date == date.Date )
                return true;
        }

        return false;
    }

    /// <summary>
    /// Enumerates the date values that can be converted from the supplied collection.
    /// </summary>
    /// <param name="values">Values to convert.</param>
    /// <returns>The converted date values.</returns>
    public static IEnumerable<DateTime> EnumerateDates( IEnumerable values )
    {
        if ( values is null )
            yield break;

        foreach ( object value in values )
        {
            if ( TryConvertToDateTime( value, out DateTime date ) )
            {
                yield return date;
            }
        }
    }

    /// <summary>
    /// Attempts to move a date by a number of days or months.
    /// </summary>
    /// <param name="date">Date to move.</param>
    /// <param name="amount">Number of periods to move.</param>
    /// <param name="byMonth">Whether <paramref name="amount"/> represents months instead of days.</param>
    /// <param name="result">Moved date, or the original date when movement exceeds the supported range.</param>
    /// <returns><see langword="true"/> when the date was moved successfully.</returns>
    public static bool TryMoveDate( DateTime date, int amount, bool byMonth, out DateTime result )
    {
        try
        {
            result = byMonth ? date.AddMonths( amount ) : date.AddDays( amount );
            return true;
        }
        catch ( ArgumentOutOfRangeException )
        {
            result = date;
            return false;
        }
    }

    /// <summary>
    /// Moves a date into the supplied month while preserving its time and nearest valid day.
    /// </summary>
    /// <param name="date">Date whose day and time should be preserved.</param>
    /// <param name="month">Target month.</param>
    /// <returns>A date within the target month.</returns>
    public static DateTime MoveIntoMonth( DateTime date, DateTime month )
    {
        int day = Math.Min( Math.Max( date.Day, 1 ), DateTime.DaysInMonth( month.Year, month.Month ) );
        return new DateTime( month.Year, month.Month, day, date.Hour, date.Minute, date.Second, date.Kind );
    }

    private static bool TryConvertToDateTime( object value, out DateTime result )
    {
        switch ( value )
        {
            case DateTime dateTime:
                result = dateTime;
                return true;
            case DateTimeOffset dateTimeOffset:
                result = dateTimeOffset.DateTime;
                return true;
            case DateOnly dateOnly:
                result = dateOnly.ToDateTime( TimeOnly.MinValue );
                return true;
            default:
                result = default;
                return false;
        }
    }

    #endregion
}