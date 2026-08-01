#region Using directives
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Provides ISO week date calculations, formatting, and parsing.
/// </summary>
internal static class WeekDateFormat
{
    #region Members

    /// <summary>
    /// Default format used by a week picker.
    /// </summary>
    public const string DefaultDisplayFormat = "yyyy-wo";

    private static readonly Regex NativeWeekRegex = new(
        @"^(?<year>\d{4})-W(?<week>\d{1,2})$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase );

    private static readonly Regex DefaultDisplayRegex = new(
        @"^(?<year>\d{4})-(?<week>\d{1,2})(?:st|nd|rd|th)?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase );

    #endregion

    #region Methods

    /// <summary>
    /// Gets the Monday that starts the ISO week containing the supplied date.
    /// </summary>
    /// <param name="date">Date contained by the week.</param>
    /// <returns>The ISO week start.</returns>
    public static DateTime GetWeekStart( DateTime date )
    {
        int offset = ( 7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday ) % 7;
        return date.Date.AddDays( -offset );
    }

    /// <summary>
    /// Gets the Sunday that ends the ISO week containing the supplied date.
    /// </summary>
    /// <param name="date">Date contained by the week.</param>
    /// <returns>The ISO week end.</returns>
    public static DateTime GetWeekEnd( DateTime date )
        => GetWeekStart( date ).AddDays( 6 );

    /// <summary>
    /// Gets the ISO week number containing the supplied date.
    /// </summary>
    /// <param name="date">Date contained by the week.</param>
    /// <returns>The ISO week number.</returns>
    public static int GetWeekNumber( DateTime date )
        => ISOWeek.GetWeekOfYear( date );

    /// <summary>
    /// Gets the ISO week-numbering year containing the supplied date.
    /// </summary>
    /// <param name="date">Date contained by the week.</param>
    /// <returns>The ISO week-numbering year.</returns>
    public static int GetWeekYear( DateTime date )
        => ISOWeek.GetYear( date );

    /// <summary>
    /// Formats a supported date value as an ISO week.
    /// </summary>
    /// <param name="value">Date value to format.</param>
    /// <param name="format">Week-aware display format.</param>
    /// <param name="culture">Culture used for literals and ordinal suffixes.</param>
    /// <returns>The formatted week, or <see langword="null"/> when the value is null.</returns>
    public static string FormatValue( object value, string format, CultureInfo culture )
    {
        if ( value is null )
            return null;

        DateTime date = value switch
        {
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset.DateTime,
            DateOnly dateOnly => dateOnly.ToDateTime( TimeOnly.MinValue ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };

        return Format( date, format, culture );
    }

    /// <summary>
    /// Formats a date using week-aware year, week-number, and ordinal tokens.
    /// </summary>
    /// <param name="date">Date contained by the week.</param>
    /// <param name="format">Week-aware format.</param>
    /// <param name="culture">Culture used for ordinal suffixes.</param>
    /// <returns>The formatted ISO week.</returns>
    public static string Format( DateTime date, string format, CultureInfo culture )
    {
        date = GetWeekStart( date );
        format = string.IsNullOrWhiteSpace( format ) ? DefaultDisplayFormat : format;
        culture ??= CultureInfo.CurrentCulture;

        if ( !IsWeekFormat( format ) )
        {
            return date.ToString( PickerDateTimeFormat.Normalize( format ), culture );
        }

        int weekYear = GetWeekYear( date );
        int weekNumber = GetWeekNumber( date );
        StringBuilder builder = new();

        for ( int index = 0; index < format.Length; )
        {
            char character = format[index];

            if ( character == '\\' && index + 1 < format.Length )
            {
                builder.Append( format[index + 1] );
                index += 2;
                continue;
            }

            if ( character is '\'' or '"' )
            {
                char quote = character;
                index++;

                while ( index < format.Length && format[index] != quote )
                {
                    builder.Append( format[index++] );
                }

                if ( index < format.Length )
                {
                    index++;
                }

                continue;
            }

            if ( character == 'w' )
            {
                int tokenStart = index++;

                while ( index < format.Length && format[index] == character )
                {
                    index++;
                }

                int tokenLength = index - tokenStart;
                bool ordinal = index < format.Length && format[index] == 'o';

                if ( ordinal )
                {
                    index++;
                    builder.Append( weekNumber.ToString( CultureInfo.InvariantCulture ) );
                    builder.Append( GetOrdinalSuffix( weekNumber, culture ) );
                }
                else
                {
                    builder.Append( tokenLength >= 2
                        ? weekNumber.ToString( "D2", CultureInfo.InvariantCulture )
                        : weekNumber.ToString( CultureInfo.InvariantCulture ) );
                }

                continue;
            }

            if ( character is 'y' or 'Y' )
            {
                int tokenStart = index++;

                while ( index < format.Length && format[index] == character )
                {
                    index++;
                }

                int tokenLength = index - tokenStart;
                builder.Append( tokenLength <= 2
                    ? ( weekYear % 100 ).ToString( "D2", CultureInfo.InvariantCulture )
                    : weekYear.ToString( "D4", CultureInfo.InvariantCulture ) );
                continue;
            }

            builder.Append( character );
            index++;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Formats a date using the normalized value required by a native week input.
    /// </summary>
    /// <param name="value">Date value contained by the week.</param>
    /// <returns>A value in <c>yyyy-Www</c> form.</returns>
    public static string FormatNativeValue( object value )
    {
        if ( value is null )
            return null;

        DateTime date = value switch
        {
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset.DateTime,
            DateOnly dateOnly => dateOnly.ToDateTime( TimeOnly.MinValue ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };

        return $"{GetWeekYear( date ):D4}-W{GetWeekNumber( date ):D2}";
    }

    /// <summary>
    /// Attempts to parse native, default, or configured week text into its ISO Monday.
    /// </summary>
    /// <param name="value">Week text to parse.</param>
    /// <param name="inputFormat">Optional input format.</param>
    /// <param name="displayFormat">Optional display format.</param>
    /// <param name="result">The parsed ISO Monday.</param>
    /// <returns><see langword="true"/> when the text represents a valid ISO week.</returns>
    public static bool TryParse( string value, string inputFormat, string displayFormat, out DateTime result )
    {
        result = default;

        if ( string.IsNullOrWhiteSpace( value ) )
            return false;

        string text = value.Trim();

        return TryParseMatch( NativeWeekRegex.Match( text ), out result )
            || TryParseMatch( DefaultDisplayRegex.Match( text ), out result )
            || TryParseFormat( text, inputFormat, out result )
            || TryParseFormat( text, displayFormat, out result );
    }

    /// <summary>
    /// Converts a week-aware input format to an Inputmask numeric mask.
    /// </summary>
    /// <param name="format">Week-aware input format.</param>
    /// <returns>The Inputmask mask.</returns>
    public static string ToInputMask( string format )
    {
        if ( string.IsNullOrWhiteSpace( format ) )
            return null;

        StringBuilder builder = new();

        for ( int index = 0; index < format.Length; )
        {
            char character = format[index];

            if ( character == '\\' && index + 1 < format.Length )
            {
                builder.Append( format[index + 1] );
                index += 2;
                continue;
            }

            if ( character is '\'' or '"' )
            {
                char quote = character;
                index++;

                while ( index < format.Length && format[index] != quote )
                {
                    builder.Append( format[index++] );
                }

                if ( index < format.Length )
                {
                    index++;
                }

                continue;
            }

            if ( character is 'y' or 'Y' )
            {
                int length = ConsumeToken( format, ref index, character );
                builder.Append( '9', length <= 2 ? 2 : 4 );
                continue;
            }

            if ( character == 'w' )
            {
                int length = ConsumeToken( format, ref index, character );
                builder.Append( length >= 2 ? "99" : "9[9]" );

                if ( index < format.Length && format[index] == 'o' )
                {
                    builder.Append( "aa" );
                    index++;
                }

                continue;
            }

            builder.Append( character );
            index++;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Determines whether the supplied format contains an unescaped week token.
    /// </summary>
    /// <param name="format">Format to inspect.</param>
    /// <returns><see langword="true"/> when the format contains a week token.</returns>
    public static bool IsWeekFormat( string format )
    {
        if ( string.IsNullOrEmpty( format ) )
            return false;

        bool inQuote = false;
        char quote = default;

        for ( int index = 0; index < format.Length; index++ )
        {
            char character = format[index];

            if ( character == '\\' )
            {
                index++;
                continue;
            }

            if ( character is '\'' or '"' )
            {
                if ( inQuote && character == quote )
                {
                    inQuote = false;
                    quote = default;
                }
                else if ( !inQuote )
                {
                    inQuote = true;
                    quote = character;
                }

                continue;
            }

            if ( !inQuote && character == 'w' )
                return true;
        }

        return false;
    }

    private static bool TryParseFormat( string value, string format, out DateTime result )
    {
        result = default;

        if ( string.IsNullOrWhiteSpace( format ) )
            return false;

        Regex regex = CreateFormatRegex( format );
        return regex is not null && TryParseMatch( regex.Match( value ), out result );
    }

    private static Regex CreateFormatRegex( string format )
    {
        StringBuilder pattern = new( "^" );
        bool hasYear = false;
        bool hasWeek = false;

        for ( int index = 0; index < format.Length; )
        {
            char character = format[index];

            if ( character == '\\' && index + 1 < format.Length )
            {
                pattern.Append( Regex.Escape( format[index + 1].ToString() ) );
                index += 2;
                continue;
            }

            if ( character is '\'' or '"' )
            {
                char quote = character;
                index++;
                StringBuilder literal = new();

                while ( index < format.Length && format[index] != quote )
                {
                    literal.Append( format[index++] );
                }

                if ( index < format.Length )
                {
                    index++;
                }

                pattern.Append( Regex.Escape( literal.ToString() ) );
                continue;
            }

            if ( character is 'y' or 'Y' )
            {
                int length = ConsumeToken( format, ref index, character );
                pattern.Append( length <= 2 ? @"(?<year>\d{2})" : @"(?<year>\d{4})" );
                hasYear = true;
                continue;
            }

            if ( character == 'w' )
            {
                int length = ConsumeToken( format, ref index, character );
                pattern.Append( length >= 2 ? @"(?<week>\d{2})" : @"(?<week>\d{1,2})" );

                if ( index < format.Length && format[index] == 'o' )
                {
                    pattern.Append( @"(?:st|nd|rd|th)?" );
                    index++;
                }

                hasWeek = true;
                continue;
            }

            pattern.Append( Regex.Escape( character.ToString() ) );
            index++;
        }

        if ( !hasYear || !hasWeek )
            return null;

        pattern.Append( '$' );
        return new Regex( pattern.ToString(), RegexOptions.CultureInvariant | RegexOptions.IgnoreCase );
    }

    private static bool TryParseMatch( System.Text.RegularExpressions.Match match, out DateTime result )
    {
        result = default;

        if ( !match.Success
             || !int.TryParse( match.Groups["year"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int year )
             || !int.TryParse( match.Groups["week"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int week ) )
        {
            return false;
        }

        if ( match.Groups["year"].Value.Length == 2 )
        {
            year = CultureInfo.CurrentCulture.Calendar.ToFourDigitYear( year );
        }

        if ( year is < 1 or > 9999 || week < 1 || week > ISOWeek.GetWeeksInYear( year ) )
            return false;

        try
        {
            result = ISOWeek.ToDateTime( year, week, DayOfWeek.Monday );
            return true;
        }
        catch ( ArgumentOutOfRangeException )
        {
            return false;
        }
    }

    private static int ConsumeToken( string format, ref int index, char character )
    {
        int start = index++;

        while ( index < format.Length && format[index] == character )
        {
            index++;
        }

        return index - start;
    }

    private static string GetOrdinalSuffix( int value, CultureInfo culture )
    {
        if ( !string.Equals( culture.TwoLetterISOLanguageName, "en", StringComparison.OrdinalIgnoreCase ) )
            return null;

        int lastTwoDigits = value % 100;

        if ( lastTwoDigits is 11 or 12 or 13 )
            return "th";

        return ( value % 10 ) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }

    #endregion
}