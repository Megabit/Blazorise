#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using Blazorise.Localization;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Parses and normalizes textual values entered into a <see cref="DatePicker{TValue}"/>.
/// </summary>
internal static class DatePickerInputParser
{
    #region Methods

    /// <summary>
    /// Attempts to normalize the supplied text to the picker's internal date format.
    /// </summary>
    /// <param name="value">Text to parse.</param>
    /// <param name="selectionMode">Active date selection mode.</param>
    /// <param name="delimiter">Delimiter separating range or multiple values.</param>
    /// <param name="inputFormat">Configured input mask format.</param>
    /// <param name="displayFormat">Configured display format.</param>
    /// <param name="dateFormat">Internal date format.</param>
    /// <param name="inputMode">Active date input mode.</param>
    /// <param name="normalizedValue">Normalized text when parsing succeeds.</param>
    /// <param name="localizer">Resources used for date names and ordinal day extensions.</param>
    /// <param name="culture">Formatting culture, defaulting to the selected language or current culture.</param>
    /// <returns><see langword="true"/> when the complete value was parsed successfully.</returns>
    public static bool TryNormalize(
        string value,
        DateInputSelectionMode selectionMode,
        string delimiter,
        string inputFormat,
        string displayFormat,
        string dateFormat,
        DateInputMode inputMode,
        out string normalizedValue,
        ITextLocalizer localizer = null,
        CultureInfo culture = null )
    {
        normalizedValue = null;

        if ( string.IsNullOrWhiteSpace( value ) )
        {
            return false;
        }

        culture ??= localizer?.SelectedCulture ?? CultureInfo.CurrentCulture;

        if ( selectionMode == DateInputSelectionMode.Single )
        {
            if ( TryParse( value, inputFormat, displayFormat, dateFormat, inputMode, culture, localizer, out var date ) )
            {
                normalizedValue = date.ToString( dateFormat, CultureInfo.InvariantCulture );
                return true;
            }

            return false;
        }

        var parts = value.Split( delimiter, StringSplitOptions.None );
        var normalizedDates = new List<string>();

        foreach ( var part in parts )
        {
            if ( !TryParse( part, inputFormat, displayFormat, dateFormat, inputMode, culture, localizer, out var date ) )
            {
                return false;
            }

            normalizedDates.Add( date.ToString( dateFormat, CultureInfo.InvariantCulture ) );
        }

        if ( selectionMode == DateInputSelectionMode.Range && normalizedDates.Count is < 1 or > 2 )
        {
            return false;
        }

        normalizedValue = string.Join( delimiter, normalizedDates );
        return true;
    }

    private static bool TryParse(
        string value,
        string inputFormat,
        string displayFormat,
        string dateFormat,
        DateInputMode inputMode,
        CultureInfo culture,
        ITextLocalizer localizer,
        out DateTime result )
    {
        result = default;

        var trimmedValue = value?.Trim();

        if ( inputMode == DateInputMode.Week )
        {
            if ( WeekDateFormat.TryParse( trimmedValue, inputFormat, displayFormat, out result ) )
            {
                return true;
            }

            var dateFormats = new List<string>();

            if ( !WeekDateFormat.IsWeekFormat( inputFormat ) )
            {
                AddFormat( dateFormats, PickerDateTimeFormat.Normalize( inputFormat ) );
            }

            if ( !WeekDateFormat.IsWeekFormat( displayFormat ) )
            {
                AddFormat( dateFormats, PickerDateTimeFormat.Normalize( displayFormat ) );
            }

            AddFormat( dateFormats, dateFormat );

            foreach ( var format in dateFormats )
            {
                if ( TryParseFormat( trimmedValue, format, culture, localizer, out result ) )
                {
                    result = WeekDateFormat.GetWeekStart( result );
                    return true;
                }
            }

            return false;
        }

        var formats = new List<string>();

        AddFormat( formats, PickerDateTimeFormat.Normalize( inputFormat ) );
        AddFormat( formats, PickerDateTimeFormat.Normalize( displayFormat ) );
        AddFormat( formats, dateFormat );

        foreach ( var format in formats )
        {
            if ( TryParseFormat( trimmedValue, format, culture, localizer, out result ) )
            {
                return true;
            }
        }

        if ( !string.IsNullOrWhiteSpace( inputFormat ) || !string.IsNullOrWhiteSpace( displayFormat ) )
        {
            return false;
        }

        return DateTime.TryParse( trimmedValue, DatePickerCulture.Resolve( culture, localizer ), DateTimeStyles.AllowWhiteSpaces, out result )
            || DateTime.TryParse( trimmedValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result );
    }

    private static bool TryParseFormat( string value, string format, CultureInfo culture, ITextLocalizer localizer, out DateTime result )
    {
        if ( PickerDateTimeFormat.TryParseExact( value, format, culture, DateTimeStyles.AllowWhiteSpaces, out result, localizer ) )
        {
            return true;
        }

        // Ordinal input must use the same language rules as the displayed date.
        return !PickerDateTimeFormat.HasOrdinalDay( format )
            && PickerDateTimeFormat.TryParseExact( value, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result, localizer );
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