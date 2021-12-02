#region Using directives
using System;
using System.Globalization;
using System.Text.Json.Serialization;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Charts
{
    public struct ChartColor
    {
        #region Constructors

        public ChartColor( byte red, byte green, byte blue )
            : this( red, green, blue, 1f )
        {
        }

        public ChartColor( byte red, byte green, byte blue, float alpha )
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public ChartColor( float red, float green, float blue )
            : this( red, green, blue, 1f )
        {
        }

        public ChartColor( float red, float green, float blue, float alpha )
        {
            R = (byte)( red * 255 );
            G = (byte)( green * 255 );
            B = (byte)( blue * 255 );
            A = alpha;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implicitly convert color to the string representation that is understood by the ChartJs.
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator string( ChartColor color ) => color.ToJsRgba();

        /// <summary>
        /// Creates the new color based on the supplied color component values.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static ChartColor FromRgba( byte red, byte green, byte blue, float alpha ) => new( red, green, blue, alpha );

        /// <summary>
        /// Creates a new color based on the supplied HTML color code.
        /// </summary>
        /// <param name="code">The HTML color code to parse</param>
        /// <returns><see cref="ChartColor"/></returns>
        public static ChartColor FromHtmlColorCode( string code )
        {
            if ( code == null )
            {
                throw new ArgumentNullException( nameof( code ) );
            }

            if ( HtmlColorCodeParser.TryParse( code, out var red, out var green, out var blue ) )
            {
                return new( red, green, blue );
            }

            throw new ArgumentException( $"The \"{code}\" doesn't represent a valid HTML color code.", nameof( code ) );
        }

        /// <summary>
        /// Converts the color to the js function call.
        /// </summary>
        /// <returns></returns>
        public string ToJsRgba() => $"rgba({R},{G},{B},{( A ?? 0 ).ToString( CultureInfo.InvariantCulture )})";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the red component value of color structure.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public byte? R { get; set; } = 0;

        /// <summary>
        /// Gets or sets the green component value of color structure.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public byte? G { get; set; } = 0;

        /// <summary>
        /// Gets or sets the blue component value of color structure.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public byte? B { get; set; } = 0;

        /// <summary>
        /// Gets or sets the alpha component value of color structure.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public float? A { get; set; } = 0;

        #endregion
    }
}
