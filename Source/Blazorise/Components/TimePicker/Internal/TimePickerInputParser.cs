#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Parses and normalizes textual values entered into a <see cref="TimePicker{TValue}"/>.
/// </summary>
internal static class TimePickerInputParser
{
    #region Methods

    /// <summary>
    /// Attempts to normalize the supplied text to the picker's internal time format.
    /// </summary>
    /// <param name="value">Text to parse.</param>
    /// <param name="displayFormat">Configured display format.</param>
    /// <param name="effectiveDisplayFormat">Resolved display format used by the picker.</param>
    /// <param name="seconds">Whether seconds are enabled.</param>
    /// <param name="timeAs24hr">Whether the picker uses a 24-hour clock.</param>
    /// <param name="isPostMeridiem">Current meridiem state used for compact 12-hour input.</param>
    /// <param name="min">Optional minimum time.</param>
    /// <param name="max">Optional maximum time.</param>
    /// <param name="normalizedValue">Normalized text when parsing succeeds.</param>
    /// <param name="result">Parsed and constrained time.</param>
    /// <returns><see langword="true"/> when the complete value was parsed successfully.</returns>
    public static bool TryNormalize(
        string value,
        string displayFormat,
        string effectiveDisplayFormat,
        bool seconds,
        bool timeAs24hr,
        bool isPostMeridiem,
        TimeSpan? min,
        TimeSpan? max,
        out string normalizedValue,
        out TimeSpan result )
    {
        normalizedValue = null;
        result = default;

        if ( string.IsNullOrWhiteSpace( value )
             || !TryParse( value, displayFormat, effectiveDisplayFormat, seconds, timeAs24hr, isPostMeridiem, out result ) )
        {
            return false;
        }

        result = TimePickerTimeUtilities.Clamp( result, min, max );
        normalizedValue = TimePickerTimeUtilities.FormatInternal( result );

        return true;
    }

    private static bool TryParse(
        string value,
        string displayFormat,
        string effectiveDisplayFormat,
        bool seconds,
        bool timeAs24hr,
        bool isPostMeridiem,
        out TimeSpan result )
    {
        result = default;

        string trimmedValue = value?.Trim();
        List<string> formats = new();

        AddFormat( formats, PickerDateTimeFormat.Normalize( displayFormat ) );
        AddFormat( formats, effectiveDisplayFormat );
        AddFormat( formats, "HH:mm:ss" );
        AddFormat( formats, "HH:mm" );
        AddFormat( formats, "H:mm" );
        AddFormat( formats, "hh:mm:ss tt" );
        AddFormat( formats, "hh:mm tt" );
        AddFormat( formats, "h:mm tt" );

        foreach ( string format in formats )
        {
            if ( DateTime.TryParseExact( trimmedValue, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsedDateTime )
                 || DateTime.TryParseExact( trimmedValue, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsedDateTime ) )
            {
                result = parsedDateTime.TimeOfDay;
                return true;
            }
        }

        if ( TryParseCompact( trimmedValue, seconds, timeAs24hr, isPostMeridiem, out result ) )
            return true;

        if ( !string.IsNullOrWhiteSpace( displayFormat ) )
            return false;

        string cultureSeparator = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
        bool hasTimeSyntax = trimmedValue.Contains( ":", StringComparison.Ordinal )
            || ( !string.IsNullOrEmpty( cultureSeparator ) && trimmedValue.Contains( cultureSeparator, StringComparison.Ordinal ) )
            || ( !string.IsNullOrEmpty( CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator ) && trimmedValue.Contains( CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator, StringComparison.OrdinalIgnoreCase ) )
            || ( !string.IsNullOrEmpty( CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator ) && trimmedValue.Contains( CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator, StringComparison.OrdinalIgnoreCase ) );

        if ( !hasTimeSyntax )
            return false;

        if ( TimeSpan.TryParse( trimmedValue, CultureInfo.CurrentCulture, out result )
             || TimeSpan.TryParse( trimmedValue, CultureInfo.InvariantCulture, out result ) )
        {
            return true;
        }

        if ( DateTime.TryParse( trimmedValue, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsed )
             || DateTime.TryParse( trimmedValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed ) )
        {
            result = parsed.TimeOfDay;
            return true;
        }

        return false;
    }

    private static bool TryParseCompact(
        string value,
        bool seconds,
        bool timeAs24hr,
        bool isPostMeridiem,
        out TimeSpan result )
    {
        result = default;

        int maximumLength = seconds ? 6 : 4;

        if ( string.IsNullOrEmpty( value )
             || value.Length > maximumLength
             || value.Any( character => character is < '0' or > '9' ) )
        {
            return false;
        }

        int hourDigits = value.Length <= 2
            ? value.Length
            : 2 - value.Length % 2;
        int hour = int.Parse( value.AsSpan( 0, hourDigits ), NumberStyles.None, CultureInfo.InvariantCulture );
        int minute = value.Length >= hourDigits + 2
            ? int.Parse( value.AsSpan( hourDigits, 2 ), NumberStyles.None, CultureInfo.InvariantCulture )
            : 0;
        int second = value.Length >= hourDigits + 4
            ? int.Parse( value.AsSpan( hourDigits + 2, 2 ), NumberStyles.None, CultureInfo.InvariantCulture )
            : 0;

        if ( hour > 23 || minute > 59 || second > 59 )
            return false;

        if ( !timeAs24hr && hour is >= 1 and <= 12 )
        {
            hour %= 12;

            if ( isPostMeridiem )
            {
                hour += 12;
            }
        }

        result = new TimeSpan( hour, minute, second );

        return true;
    }

    private static void AddFormat( ICollection<string> formats, string format )
    {
        if ( !string.IsNullOrWhiteSpace( format ) && !formats.Contains( format ) )
        {
            formats.Add( format );
        }
    }

    #endregion
}