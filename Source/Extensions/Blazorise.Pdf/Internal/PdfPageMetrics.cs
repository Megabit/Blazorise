#region Using directives
using System;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Provides standard PDF page sizes and unit conversion helpers.
/// </summary>
public static class PdfPageMetrics
{
    #region Members

    /// <summary>
    /// A4 page width in points.
    /// </summary>
    public const double A4Width = 595.275590551;

    /// <summary>
    /// A4 page height in points.
    /// </summary>
    public const double A4Height = 841.88976378;

    /// <summary>
    /// Letter page width in points.
    /// </summary>
    public const double LetterWidth = 612;

    /// <summary>
    /// Letter page height in points.
    /// </summary>
    public const double LetterHeight = 792;

    private const double PointsPerInch = 72;

    private const double MillimetersPerInch = 25.4;

    #endregion

    #region Methods

    /// <summary>
    /// Converts a value from the supplied unit to points.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="unit">The measurement unit.</param>
    /// <returns>The value converted to PDF points.</returns>
    public static double ToPoints( double value, PdfUnit unit )
    {
        return unit switch
        {
            PdfUnit.Inch => value * PointsPerInch,
            PdfUnit.Millimeter => value * PointsPerInch / MillimetersPerInch,
            PdfUnit.Centimeter => value * PointsPerInch * 10 / MillimetersPerInch,
            _ => value,
        };
    }

    /// <summary>
    /// Resolves page dimensions in points.
    /// </summary>
    /// <param name="size">The standard page size.</param>
    /// <param name="orientation">The page orientation.</param>
    /// <param name="width">The custom page width.</param>
    /// <param name="height">The custom page height.</param>
    /// <returns>The resolved page width and height in points.</returns>
    public static (double Width, double Height) Resolve( PdfPageSize size, PdfOrientation orientation, double width = 0, double height = 0 )
    {
        (double Width, double Height) result = size switch
        {
            PdfPageSize.Letter => (LetterWidth, LetterHeight),
            PdfPageSize.Custom when width > 0 && height > 0 => (width, height),
            _ => (A4Width, A4Height),
        };

        return orientation == PdfOrientation.Landscape
            ? (Math.Max( result.Width, result.Height ), Math.Min( result.Width, result.Height ))
            : (Math.Min( result.Width, result.Height ), Math.Max( result.Width, result.Height ));
    }

    #endregion
}