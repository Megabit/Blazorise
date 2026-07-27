#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Pdf.Internal;

internal sealed class PdfTrueTypeFontMetrics : IPdfFontMetrics
{
    #region Members

    internal const int DefaultGlyphWidth = 500;

    private readonly IReadOnlyDictionary<int, int> glyphsByCodePoint;
    private readonly IReadOnlyDictionary<int, int> glyphWidths;

    #endregion

    #region Constructors

    internal PdfTrueTypeFontMetrics( IReadOnlyDictionary<int, int> glyphsByCodePoint, IReadOnlyDictionary<int, int> glyphWidths )
    {
        this.glyphsByCodePoint = glyphsByCodePoint;
        this.glyphWidths = glyphWidths;
    }

    #endregion

    #region Methods

    public double MeasureTextWidth( string text, double fontSize )
    {
        if ( string.IsNullOrEmpty( text ) )
            return 0;

        double width = 0;

        foreach ( int codePoint in EnumerateCodePoints( text ) )
        {
            width += GetGlyphWidth( GetGlyphId( codePoint ) );
        }

        return width * fontSize / 1000d;
    }

    public int GetGlyphWidth( int glyphId )
    {
        return glyphWidths is not null && glyphWidths.TryGetValue( glyphId, out int width )
            ? width
            : DefaultGlyphWidth;
    }

    internal int GetGlyphId( int codePoint )
    {
        return glyphsByCodePoint is not null && glyphsByCodePoint.TryGetValue( codePoint, out int glyphId )
            ? glyphId
            : 0;
    }

    private static IEnumerable<int> EnumerateCodePoints( string text )
    {
        for ( int i = 0; i < text.Length; i++ )
        {
            char character = text[i];

            if ( char.IsHighSurrogate( character ) && i + 1 < text.Length && char.IsLowSurrogate( text[i + 1] ) )
            {
                yield return char.ConvertToUtf32( character, text[++i] );
                continue;
            }

            yield return character;
        }
    }

    #endregion
}