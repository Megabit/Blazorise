#region Using directives
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Defauult implementation of <see cref="IDateTimeFormatConverter"/>.
/// </summary>
public class DateTimeFormatConverter : IDateTimeFormatConverter
{
    #region Members

    private static readonly Dictionary<string, string> FormatTokens = new()
    {
        { "d", "j" },       // Day of the month without leading zeros, 1 to 31.
        { "dd", "d" },      // Day of the month, 2 digits with leading zeros, 01 to 31.
        { "ddd", "D" },     // A textual representation of a day, Mon through Sun.
        { "dddd", "l" },    // A full textual representation of the day of the week, Sunday through Saturday.

        { "M", "n" },       // Numeric representation of a month, without leading zeros, 1 through 12.
        { "MM", "m" },      // Numeric representation of a month, with leading zero, 01 through 12.
        { "MMM", "M" },     // A short textual representation of a month, Jan through Dec.
        { "MMMM", "F" },    // A full textual representation of a month, January through December.

        { "y", "y" },       // A two digit representation of a year. 99 or 03.
        { "yy", "y" },      // -||-

        { "yyy", "Y" },     // A full numeric representation of a year, 4 digits, 1999 or 2003.
        { "yyyy", "Y" },    // -||-
        { "yyyyy", "Y" },   // -||-

        { "H", "H" },       // Hours (24 hours), 00 to 23.
        { "HH", "H" },      // -||-
        { "h", "h" },       // Hours, 1 to 12.
        { "hh", "G" },      // Hours, 2 digits with leading zeros, 01 to 12.

        { "m", "i" },       // Minutes, 00 to 59.
        { "mm", "i" },      // -||-

        { "s", "s" },       // Seconds, 0, 1 to 59.
        { "ss", "S" },      // Seconds, 2 digits, 00 to 59.

        { "t", "K" },       // AM/PM designator.
        { "tt", "K" },      // -||-
    };

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Convert( string format )
    {
        if ( string.IsNullOrEmpty( format ) || format.Length == 0 )
            return format;

        var tokens = ParseTokens( format );

        var endFormat = new StringBuilder();

        foreach ( var token in tokens )
        {
            if ( FormatTokens.TryGetValue( token, out var internalToken ) )
                endFormat.Append( internalToken );
            else
                endFormat.Append( token );
        }

        return endFormat.ToString();
    }

    private IList<string> ParseTokens( string format )
    {
        var tokens = new List<string>();

        string tempToken = null;
        char? last = null;

        foreach ( var c in format )
        {
            if ( !char.IsLetter( c ) )
            {
                if ( tempToken is not null )
                    tokens.Add( tempToken );

                tokens.Add( c.ToString() );

                tempToken = null;
                last = null;
                continue;
            }

            if ( last is null || ( last is not null && last == c ) )
            {
                tempToken ??= string.Empty;

                tempToken += c;
            }

            last = c;
        }

        if ( !string.IsNullOrEmpty( tempToken ) )
        {
            tokens.Add( tempToken );
        }

        return tokens;
    }

    #endregion
}