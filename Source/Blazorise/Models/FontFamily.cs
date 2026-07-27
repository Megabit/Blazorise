#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines a font family that can be used by Blazorise renderers and exporters.
/// </summary>
public sealed class FontFamily
{
    #region Methods

    /// <summary>
    /// Resolves the best source for the requested font style.
    /// </summary>
    /// <param name="bold">Whether a bold source is preferred.</param>
    /// <param name="italic">Whether an italic source is preferred.</param>
    /// <returns>The best matching font source.</returns>
    public FontSource ResolveSource( bool bold = false, bool italic = false )
    {
        return ( bold, italic ) switch
        {
            ( true, true ) => BoldItalic ?? Bold ?? Italic ?? Regular,
            ( true, false ) => Bold ?? Regular,
            ( false, true ) => Italic ?? Regular,
            _ => Regular,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Font family name used by components.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// User-facing font family name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// CSS font-family value used by browser-based rendering.
    /// </summary>
    public string CssFamily { get; set; }

    /// <summary>
    /// Regular font source.
    /// </summary>
    public FontSource Regular { get; set; }

    /// <summary>
    /// Bold font source.
    /// </summary>
    public FontSource Bold { get; set; }

    /// <summary>
    /// Italic font source.
    /// </summary>
    public FontSource Italic { get; set; }

    /// <summary>
    /// Bold italic font source.
    /// </summary>
    public FontSource BoldItalic { get; set; }

    /// <summary>
    /// Indicates whether the font is visible in UI selectors.
    /// </summary>
    public bool Visible { get; set; } = true;

    #endregion
}