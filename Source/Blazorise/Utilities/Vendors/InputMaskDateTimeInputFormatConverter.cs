#region Using directives
using System.Collections.Generic;
using Blazorise.Vendors;
#endregion

namespace Blazorise.Utilities.Vendors;

/// <summary>
/// Default implementation of <see cref="IInputMaskDateTimeInputFormatConverter"/>.
/// </summary>
public class InputMaskDateTimeInputFormatConverter : BaseDateTimeFormatConverter, IInputMaskDateTimeInputFormatConverter
{
    #region Properties

    private static readonly Dictionary<string, string> FormatTokensMap = new()
    {
        { "d", "d" },           // Day of the month without leading zeros, 1 to 31.
        { "dd", "dd" },         // Day of the month, 2 digits with leading zeros, 01 to 31.
        { "ddd", "ddd" },       // A textual representation of a day, Mon through Sun.
        { "dddd", "dddd" },     // A full textual representation of the day of the week, Sunday through Saturday.

        { "M", "m" },           // Numeric representation of a month, without leading zeros, 1 through 12.
        { "MM", "mm" },         // Numeric representation of a month, with leading zero, 01 through 12.
        { "MMM", "mmm" },       // A short textual representation of a month, Jan through Dec.
        { "MMMM", "mmmm" },     // A full textual representation of a month, January through December.

        { "y", "yy" },          // A two digit representation of a year. 99 or 03.
        { "yy", "yy" },         // -||-

        { "yyy", "yyyy" },      // A full numeric representation of a year, 4 digits, 1999 or 2003.
        { "yyyy", "yyyy" },     // -||-
        { "yyyyy", "yyyy" },    // -||-

        { "H", "H" },           // The hour, using a 24-hour clock from 0 to 23.
        { "HH", "HH" },         // The hour, using a 24-hour clock from 00 to 23.
        { "h", "h" },           // The hour, using a 12-hour clock from 1 to 12.
        { "hh", "hh" },         // The hour, using a 12-hour clock from 01 to 12.

        { "m", "M" },           // The minute, from 0 to 59.
        { "mm", "MM" },         // The minute, from 00 to 59.

        { "s", "s" },           // The second, from 0 to 59.
        { "ss", "ss" },         // 	The second, from 00 to 59.

        { "t", "T" },           // AM/PM designator.
        { "tt", "TT" },         // -||-
    };

    /// <inheritdoc/>
    protected override Dictionary<string, string> FormatTokens => FormatTokensMap;

    #endregion
}