#region Using directives
using System;
using System.ComponentModel;
using System.Globalization;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Concrete RGB and alpha values resolved from a report color.
/// </summary>
/// <param name="Red">Red component from 0 to 255.</param>
/// <param name="Green">Green component from 0 to 255.</param>
/// <param name="Blue">Blue component from 0 to 255.</param>
/// <param name="Alpha">Color opacity from 0 to 1.</param>
public readonly record struct ReportResolvedColor( byte Red, byte Green, byte Blue, double Alpha )
{
    /// <summary>
    /// Converts the resolved color to a CSS-compatible color value.
    /// </summary>
    /// <returns>A CSS color value, or <c>null</c> when the color is not explicitly applied.</returns>
    public string ToCssString()
    {
        if ( Alpha < 0 )
            return null;

        return Alpha >= 1
            ? FormattableString.Invariant( $"#{Red:X2}{Green:X2}{Blue:X2}" )
            : FormattableString.Invariant( $"rgba({Red}, {Green}, {Blue}, {Alpha.ToString( "0.###", CultureInfo.InvariantCulture )})" );
    }
}