namespace Blazorise.Pdf.Internal;

internal sealed class PdfStandardFontMetrics : IPdfFontMetrics
{
    #region Members

    private const int FallbackWidth = 500;

    private readonly string baseFontName;

    private static readonly int[] HelveticaWidths =
    [
        278, 278, 355, 556, 556, 889, 667, 222, 333, 333, 389, 584, 278, 333, 278, 278,
        556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 278, 278, 584, 584, 584, 556,
        1015, 667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778,
        667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 278, 278, 278, 469, 556,
        222, 556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556,
        556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500, 334, 260, 334, 584
    ];

    private static readonly int[] HelveticaBoldWidths =
    [
        278, 333, 474, 556, 556, 889, 722, 278, 333, 333, 389, 584, 278, 333, 278, 278,
        556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 333, 333, 584, 584, 584, 611,
        975, 722, 722, 722, 722, 667, 611, 778, 722, 278, 556, 722, 611, 833, 722, 778,
        667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 333, 278, 333, 584, 556,
        278, 556, 611, 556, 611, 556, 333, 611, 611, 278, 278, 556, 278, 889, 611, 611,
        611, 611, 389, 556, 333, 611, 556, 778, 556, 556, 500, 389, 280, 389, 584
    ];

    private static readonly int[] TimesRomanWidths =
    [
        250, 333, 408, 500, 500, 833, 778, 180, 333, 333, 500, 564, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 278, 278, 564, 564, 564, 444,
        921, 722, 667, 667, 722, 611, 556, 722, 722, 333, 389, 722, 611, 889, 722, 722,
        556, 722, 667, 556, 611, 722, 722, 944, 722, 722, 611, 333, 278, 333, 469, 500,
        333, 444, 500, 444, 500, 444, 333, 500, 500, 278, 278, 500, 278, 778, 500, 500,
        500, 500, 333, 389, 278, 500, 500, 722, 500, 500, 444, 480, 200, 480, 541
    ];

    private static readonly int[] TimesBoldWidths =
    [
        250, 333, 555, 500, 500, 1000, 833, 278, 333, 333, 500, 570, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 570, 570, 570, 500,
        930, 722, 667, 722, 722, 667, 611, 778, 778, 389, 500, 778, 667, 944, 722, 778,
        611, 778, 722, 556, 667, 722, 722, 1000, 722, 722, 667, 333, 278, 333, 581, 500,
        333, 500, 556, 444, 556, 444, 333, 500, 556, 278, 333, 556, 278, 833, 556, 500,
        556, 556, 444, 389, 333, 556, 500, 722, 500, 500, 444, 394, 220, 394, 520
    ];

    private static readonly int[] TimesItalicWidths =
    [
        250, 333, 420, 500, 500, 833, 778, 214, 333, 333, 500, 675, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 675, 675, 675, 500,
        920, 611, 611, 667, 722, 611, 611, 722, 722, 333, 444, 667, 556, 833, 667, 722,
        611, 722, 611, 500, 556, 722, 611, 833, 611, 556, 556, 389, 278, 389, 422, 500,
        333, 500, 500, 444, 500, 444, 278, 500, 500, 278, 278, 444, 278, 722, 500, 500,
        500, 500, 389, 389, 278, 500, 444, 667, 444, 444, 389, 400, 275, 400, 541
    ];

    private static readonly int[] TimesBoldItalicWidths =
    [
        250, 389, 555, 500, 500, 833, 778, 278, 333, 333, 500, 570, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 570, 570, 570, 500,
        832, 667, 667, 667, 722, 667, 667, 722, 778, 389, 500, 667, 611, 889, 722, 722,
        611, 722, 667, 556, 611, 722, 667, 889, 667, 611, 611, 333, 278, 333, 570, 500,
        333, 500, 500, 444, 500, 444, 333, 500, 556, 278, 278, 500, 278, 778, 556, 500,
        500, 500, 389, 389, 278, 556, 444, 667, 500, 444, 389, 348, 220, 348, 570
    ];

    #endregion

    #region Constructors

    internal PdfStandardFontMetrics( string baseFontName )
    {
        this.baseFontName = baseFontName;
    }

    #endregion

    #region Methods

    public double MeasureTextWidth( string text, double fontSize )
    {
        if ( string.IsNullOrEmpty( text ) )
            return 0;

        double width = 0;

        foreach ( char character in text )
        {
            if ( TryGetType1TextByte( character, out byte value ) )
                width += GetGlyphWidth( value );
            else
                width += GetGlyphWidth( '?' );
        }

        return width * fontSize / 1000d;
    }

    public int GetGlyphWidth( int glyphId )
    {
        if ( IsCourierFont( baseFontName ) )
            return 600;

        int[] widths = ResolveWidths( baseFontName );

        return glyphId is >= 32 and <= 126
            ? widths[glyphId - 32]
            : FallbackWidth;
    }

    private static int[] ResolveWidths( string baseFontName )
    {
        return baseFontName switch
        {
            "Helvetica-Bold" or "Helvetica-BoldOblique" => HelveticaBoldWidths,
            "Times-Bold" => TimesBoldWidths,
            "Times-Italic" => TimesItalicWidths,
            "Times-BoldItalic" => TimesBoldItalicWidths,
            "Times-Roman" => TimesRomanWidths,
            _ => HelveticaWidths,
        };
    }

    private static bool IsCourierFont( string baseFontName )
    {
        return baseFontName is "Courier" or "Courier-Bold" or "Courier-Oblique" or "Courier-BoldOblique";
    }

    private static bool TryGetType1TextByte( char character, out byte value )
    {
        if ( character <= 0x7F )
        {
            value = (byte)character;
            return true;
        }

        if ( character >= 0xA0 && character <= 0xFF )
        {
            value = (byte)character;
            return true;
        }

        value = character switch
        {
            '\u20AC' => 0x80,
            '\u201A' => 0x82,
            '\u0192' => 0x83,
            '\u201E' => 0x84,
            '\u2026' => 0x85,
            '\u2020' => 0x86,
            '\u2021' => 0x87,
            '\u02C6' => 0x88,
            '\u2030' => 0x89,
            '\u0160' => 0x8A,
            '\u2039' => 0x8B,
            '\u0152' => 0x8C,
            '\u017D' => 0x8E,
            '\u010D' => 0x81,
            '\u0110' => 0x83,
            '\u0107' => 0x8D,
            '\u0111' => 0x8F,
            '\u010C' => 0x90,
            '\u2018' => 0x91,
            '\u2019' => 0x92,
            '\u201C' => 0x93,
            '\u201D' => 0x94,
            '\u2022' => 0x95,
            '\u2013' => 0x96,
            '\u2014' => 0x97,
            '\u02DC' => 0x98,
            '\u2122' => 0x99,
            '\u0161' => 0x9A,
            '\u203A' => 0x9B,
            '\u0153' => 0x9C,
            '\u0106' => 0x9D,
            '\u017E' => 0x9E,
            '\u0178' => 0x9F,
            _ => 0,
        };

        return value != 0;
    }

    #endregion
}