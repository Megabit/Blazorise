#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <param name="normalizedValue">Normalized text when parsing succeeds.</param>
    /// <returns><see langword="true"/> when the complete value was parsed successfully.</returns>
    public static bool TryNormalize(
        string value,
        DateInputSelectionMode selectionMode,
        string delimiter,
        string inputFormat,
        string displayFormat,
        string dateFormat,
        out string normalizedValue )
    {
        normalizedValue = null;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        if ( selectionMode == DateInputSelectionMode.Single )
        {
            if ( TryParse( value, inputFormat, displayFormat, dateFormat, out DateTime date ) )
            {
                normalizedValue = date.ToString( dateFormat, CultureInfo.InvariantCulture );
                return true;
            }

            return false;
        }

        string[] parts = value.Split( delimiter, StringSplitOptions.None );
        List<string> normalizedDates = new();

        foreach ( string part in parts )
        {
            if ( !TryParse( part, inputFormat, displayFormat, dateFormat, out DateTime date ) )
                return false;

            normalizedDates.Add( date.ToString( dateFormat, CultureInfo.InvariantCulture ) );
        }

        if ( selectionMode == DateInputSelectionMode.Range && normalizedDates.Count is < 1 or > 2 )
            return false;

        normalizedValue = string.Join( delimiter, normalizedDates );
        return true;
    }

    private static bool TryParse(
        string value,
        string inputFormat,
        string displayFormat,
        string dateFormat,
        out DateTime result )
    {
        result = default;

        string trimmedValue = value?.Trim();
        List<string> formats = new();

        AddFormat( formats, PickerDateTimeFormat.Normalize( inputFormat ) );
        AddFormat( formats, PickerDateTimeFormat.Normalize( displayFormat ) );
        AddFormat( formats, dateFormat );

        foreach ( string format in formats )
        {
            if ( DateTime.TryParseExact( trimmedValue, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result )
                 || DateTime.TryParseExact( trimmedValue, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result ) )
            {
                return true;
            }
        }

        if ( !string.IsNullOrWhiteSpace( inputFormat ) || !string.IsNullOrWhiteSpace( displayFormat ) )
            return false;

        return DateTime.TryParse( trimmedValue, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result )
            || DateTime.TryParse( trimmedValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result );
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