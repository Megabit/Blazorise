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
        public const string InternalDateFormat = "dd.MM.yyyy";

        /// <summary>
        /// Default date format.
        /// </summary>
        public const string ExternalDateFormat = "yyyy-MM-dd";

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

        public static DateTime? TryParseDate( string value )
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                return null;

            if ( DateTime.TryParseExact( value, SupportedDateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt ) )
                return dt;

            if ( DateTimeOffset.TryParse( value, out var dto ) )
            {
                //Console.WriteLine( $"Failsafe parse: {dto}" );
                return dto.DateTime;
            }

            //Console.WriteLine( $"Date format is not supported: {value}" );

            return null;
        }
    }
}
