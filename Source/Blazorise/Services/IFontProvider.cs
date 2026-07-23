#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Provides registered Blazorise font families.
/// </summary>
public interface IFontProvider
{
    /// <summary>
    /// Gets all registered font families.
    /// </summary>
    /// <returns>Registered font families.</returns>
    IReadOnlyList<FontFamily> GetFonts();

    /// <summary>
    /// Resolves the requested font family.
    /// </summary>
    /// <param name="family">Font family name.</param>
    /// <returns>The resolved font family.</returns>
    FontFamily Resolve( string family );

    /// <summary>
    /// Resolves the requested font source.
    /// </summary>
    /// <param name="family">Font family name.</param>
    /// <param name="bold">Whether a bold source is preferred.</param>
    /// <param name="italic">Whether an italic source is preferred.</param>
    /// <returns>The resolved font source.</returns>
    FontSource ResolveSource( string family, bool bold = false, bool italic = false );
}