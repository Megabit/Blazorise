#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Holds the build time of the assembly.
    /// </summary>
    [AttributeUsage( AttributeTargets.Assembly )]
    public class BuildDateTimeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the build date.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// A default constructor.
        /// </summary>
        /// <param name="date">Build date value.</param>
        public BuildDateTimeAttribute( string date )
        {
            Date = date;
        }

        /// <summary>
        /// Gets the actual build time stored in the <see cref="BuildDateTimeAttribute"/>.
        /// </summary>
        /// <returns>Build date value.</returns>
        public static DateTime? GetDateTime()
        {
            var attr = Attribute.GetCustomAttribute( typeof( BuildDateTimeAttribute ).Assembly, typeof( BuildDateTimeAttribute ) ) as BuildDateTimeAttribute;

            if ( DateTime.TryParseExact( attr?.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt ) )
                return dt;
            else
                return null;
        }
    }
}
