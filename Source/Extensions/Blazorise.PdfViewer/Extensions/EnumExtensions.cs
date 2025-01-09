namespace Blazorise.PdfViewer;

/// <summary>
/// Provides extension methods for working with PDF orientation enumerations.
/// </summary>
internal static class EnumExtensions
{
    /// <summary>
    /// Converts a <see cref="PdfOrientation"/> value to a corresponding rotation angle in degrees.
    /// </summary>
    /// <param name="orientation">The <see cref="PdfOrientation"/> to convert.</param>
    /// <returns>The rotation angle in degrees. Returns <c>90</c> (clockwise) for landscape orientation and <c>0</c> for portrait orientation.</returns>
    public static double ToRotation( this PdfOrientation orientation )
    {
        return orientation switch
        {
            PdfOrientation.Landscape => 90,
            _ => 0,
        };
    }
}