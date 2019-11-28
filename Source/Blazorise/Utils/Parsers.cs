#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utils
{
    public static class Parsers
    {
        /// <summary>
        /// Internal date format.
        /// </summary>
        public const string InternalDateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Default date format.
        /// </summary>
        public const string ExternalDateFormat = "dd.MM.yyyy";

        /// <summary>
        /// Possible date formats.
        /// </summary>
        public static readonly string[] SupportedDateFormats = new string[]
        {
            InternalDateFormat,
            ExternalDateFormat,
            "yyyy-MM-ddTHH:mm",
            "o", // a string representing UTC
        };

        public static bool TryParseDate( string value, out DateTime? result )
        {
            if ( string.IsNullOrWhiteSpace( value ) )
            {
                result = null;
                return false;
            }

            if ( DateTime.TryParseExact( value, SupportedDateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt ) )
            {
                result = dt;
                return true;
            }

            if ( DateTimeOffset.TryParse( value, out var dto ) )
            {
                result = dto.DateTime;
                return true;
            }

            result = null;
            return false;
        }
    }
}
