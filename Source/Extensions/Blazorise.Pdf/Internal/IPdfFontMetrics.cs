#region Using directives
using System.Threading;
#endregion

namespace Blazorise.Pdf.Internal;

internal interface IPdfFontMetrics
{
    double MeasureTextWidth( string text, double fontSize, CancellationToken cancellationToken );

    int GetGlyphWidth( int glyphId );
}