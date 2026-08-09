#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Pdf.Internal;

internal sealed class PdfDocumentFontProvider : IFontProvider
{
    #region Members

    private readonly IReadOnlyList<FontFamily> fonts;
    private readonly IFontProvider fallbackProvider;

    #endregion

    #region Constructors

    internal PdfDocumentFontProvider( IReadOnlyList<FontFamily> fonts, IFontProvider fallbackProvider )
    {
        this.fonts = fonts ?? [];
        this.fallbackProvider = fallbackProvider;
    }

    #endregion

    #region Methods

    public IReadOnlyList<FontFamily> GetFonts()
    {
        List<FontFamily> resolvedFonts = [.. fonts];

        if ( fallbackProvider is null )
            return resolvedFonts;

        IReadOnlyList<FontFamily> fallbackFonts = fallbackProvider.GetFonts();

        if ( fallbackFonts is null )
            return resolvedFonts;

        foreach ( FontFamily font in fallbackFonts )
        {
            resolvedFonts.Add( font );
        }

        return resolvedFonts;
    }

    public FontFamily Resolve( string family )
    {
        return Find( family ) ?? fallbackProvider?.Resolve( family );
    }

    public FontSource ResolveSource( string family, bool bold = false, bool italic = false )
    {
        return Resolve( family )?.ResolveSource( bold, italic );
    }

    private FontFamily Find( string family )
    {
        if ( string.IsNullOrWhiteSpace( family ) )
            return null;

        foreach ( FontFamily font in fonts )
        {
            if ( string.Equals( font?.Name, family, StringComparison.OrdinalIgnoreCase ) )
                return font;
        }

        return null;
    }

    #endregion
}