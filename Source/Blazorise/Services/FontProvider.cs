#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

/// <summary>
/// Default Blazorise font provider.
/// </summary>
public sealed class FontProvider : IFontProvider
{
    #region Members

    private readonly BlazoriseOptions options;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the Blazorise font provider.
    /// </summary>
    /// <param name="options">Blazorise options.</param>
    public FontProvider( BlazoriseOptions options )
    {
        this.options = options;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public IReadOnlyList<FontFamily> GetFonts()
    {
        return options.Fonts.Families.ToList();
    }

    /// <inheritdoc />
    public FontFamily Resolve( string family )
    {
        return Find( family )
               ?? Find( options.Fonts.DefaultFamily )
               ?? Find( options.Fonts.FallbackFamily )
               ?? Find( Fonts.Helvetica )
               ?? options.Fonts.Families.FirstOrDefault();
    }

    /// <inheritdoc />
    public FontSource ResolveSource( string family, bool bold = false, bool italic = false )
    {
        return Resolve( family )?.ResolveSource( bold, italic );
    }

    private FontFamily Find( string family )
    {
        if ( string.IsNullOrWhiteSpace( family ) )
            return null;

        foreach ( FontFamily font in options.Fonts.Families )
        {
            if ( string.Equals( font.Name, family, StringComparison.OrdinalIgnoreCase ) )
                return font;
        }

        return null;
    }

    #endregion
}