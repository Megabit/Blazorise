namespace Blazorise.Pdf.Internal;

internal interface IPdfFontMetrics
{
    double MeasureTextWidth( string text, double fontSize );

    int GetGlyphWidth( int glyphId );
}