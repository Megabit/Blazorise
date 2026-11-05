#region Using directives
using System;
using System.Globalization;
using System.Text;
using Blazorise.Localization;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Converts the public picker format syntax into an equivalent .NET custom format.
/// </summary>
/// <remarks>
/// DatePicker and TimePicker historically converted their public .NET-like formats before formatting.
/// Some of those tokens differ subtly from direct <c>DateTime.ToString</c> behavior. This converter
/// preserves the established picker behavior.
/// </remarks>
internal static class PickerDateTimeFormat
{
    #region Methods

    /// <summary>
    /// Converts a public picker format into a .NET custom format with the same output semantics as the
    /// former picker format converter.
    /// </summary>
    public static string Normalize( string format )
        => ConvertFormat( format, true, null, out _ );

    /// <summary>
    /// Formats a date using a normalized picker format, including date-dependent extensions.
    /// </summary>
    public static string FormatDateValue( object value, string format, CultureInfo culture, ITextLocalizer localizer = null )
    {
        culture = DatePickerCulture.Resolve( culture, localizer );

        return value switch
        {
            null => null,
            DateTime date => date.ToString( ResolveDateFormat( format, date, culture, localizer ), culture ),
            DateTimeOffset date => date.ToString( ResolveDateFormat( format, date.DateTime, culture, localizer ), culture ),
            DateOnly date => date.ToString( ResolveDateFormat( format, date.ToDateTime( TimeOnly.MinValue ), culture, localizer ), culture ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };
    }

    /// <summary>
    /// Resolves date extensions to native day specifiers with escaped localized affixes.
    /// </summary>
    private static string ResolveDateFormat( string format, DateTime date, CultureInfo culture, ITextLocalizer localizer )
    {
        if ( format is null || !format.Contains( '{' ) )
        {
            return format;
        }

        var day = culture.DateTimeFormat.Calendar.GetDayOfMonth( date );
        var ordinalDayFormat = OrdinalDateFormat.GetDayFormat( day, culture, localizer );

        return ConvertFormat( format, false, ordinalDayFormat, out _ );
    }

    /// <summary>
    /// Parses a normalized picker format and validates the localized ordinal pattern against the parsed day.
    /// </summary>
    public static bool TryParseExact( string value, string format, CultureInfo culture, DateTimeStyles styles, out DateTime result, ITextLocalizer localizer = null )
    {
        culture = DatePickerCulture.Resolve( culture, localizer );

        if ( format is null || !format.Contains( '{' ) )
        {
            return DateTime.TryParseExact( value, format, culture, styles, out result );
        }

        var numericFormat = ConvertFormat( format, false, "%d", out var hasOrdinalDay );

        if ( !hasOrdinalDay )
        {
            return DateTime.TryParseExact( value, numericFormat, culture, styles, out result );
        }

        foreach ( var dayFormat in OrdinalDateFormat.GetDayFormats( culture, localizer ) )
        {
            var ordinalFormat = ConvertFormat( format, false, dayFormat, out _ );

            if ( DateTime.TryParseExact( value, ordinalFormat, culture, styles, out result )
                && OrdinalDateFormat.GetDayFormat( culture.DateTimeFormat.Calendar.GetDayOfMonth( result ), culture, localizer ) == dayFormat )
            {
                return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the format contains an unquoted ordinal day extension.
    /// </summary>
    public static bool HasOrdinalDay( string format )
    {
        if ( format is null || !format.Contains( '{' ) )
        {
            return false;
        }

        ConvertFormat( format, false, null, out var hasOrdinalDay );
        return hasOrdinalDay;
    }

    private static string ConvertFormat( string format, bool normalizeTokens, string ordinalDayFormat, out bool hasOrdinalDay )
    {
        hasOrdinalDay = false;

        if ( string.IsNullOrEmpty( format ) )
        {
            return format;
        }

        var builder = new StringBuilder();

        for ( var index = 0; index < format.Length; )
        {
            var character = format[index];

            if ( character == '\\' && index + 1 < format.Length )
            {
                builder.Append( character );
                builder.Append( format[index + 1] );
                index += 2;
                continue;
            }

            if ( character is '\'' or '"' )
            {
                var quote = character;
                builder.Append( character );
                index++;

                while ( index < format.Length )
                {
                    var literalCharacter = format[index++];
                    builder.Append( literalCharacter );

                    if ( literalCharacter == '\\' && index < format.Length )
                    {
                        builder.Append( format[index++] );
                        continue;
                    }

                    if ( literalCharacter == quote )
                    {
                        break;
                    }
                }

                continue;
            }

            if ( character == '{' )
            {
                var end = format.IndexOf( '}', index + 1 );

                if ( end < 0 && format.IndexOf( ':', index + 1 ) >= 0 )
                {
                    throw new FormatException( "The picker format extension is missing its closing brace." );
                }

                if ( end >= 0 )
                {
                    var extension = format.Substring( index + 1, end - index - 1 );
                    var separator = extension.IndexOf( ':' );

                    if ( separator >= 0 )
                    {
                        var part = extension[..separator];
                        var modifier = extension[( separator + 1 )..];

                        switch ( (part, modifier) )
                        {
                            case ("day", "ordinal"):
                                hasOrdinalDay = true;
                                builder.Append( ordinalDayFormat ?? format.Substring( index, end - index + 1 ) );
                                break;
                            default:
                                throw new FormatException( $"Unsupported picker format extension '{{{extension}}}'." );
                        }

                        index = end + 1;
                        continue;
                    }
                }
            }

            if ( !normalizeTokens || !char.IsLetter( character ) )
            {
                builder.Append( character );
                index++;
                continue;
            }

            var tokenStart = index;

            while ( index < format.Length && format[index] == character )
            {
                index++;
            }

            var tokenLength = index - tokenStart;
            builder.Append( NormalizeToken( character, tokenLength ) );
        }

        return builder.ToString();
    }

    private static string NormalizeToken( char character, int length )
    {
        return character switch
        {
            'd' => length switch
            {
                1 => "%d",
                2 => "dd",
                3 => "ddd",
                _ => "dddd",
            },
            'M' => length switch
            {
                1 => "%M",
                2 => "MM",
                3 => "MMM",
                _ => "MMMM",
            },
            'y' => length <= 2 ? "yy" : "yyyy",
            'H' => "HH",
            'h' => length == 1 ? "%h" : "hh",
            'm' => "mm",
            's' => length == 1 ? "%s" : "ss",
            't' => "tt",
            _ => new string( character, length ),
        };
    }

    #endregion
}