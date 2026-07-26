#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines a font source used by Blazorise features that render or export text.
/// </summary>
public sealed class FontSource
{
    #region Methods

    /// <summary>
    /// Creates a font source from raw font bytes.
    /// </summary>
    /// <param name="data">Font bytes.</param>
    /// <param name="format">Font format.</param>
    /// <returns>A font source.</returns>
    public static FontSource FromBytes( byte[] data, FontFormat format = FontFormat.TrueType )
    {
        return new()
        {
            Data = data,
            Format = format,
        };
    }

    /// <summary>
    /// Creates a font source from a local font file.
    /// </summary>
    /// <param name="fileName">Font file name.</param>
    /// <param name="format">Font format.</param>
    /// <returns>A font source.</returns>
    public static FontSource FromFile( string fileName, FontFormat format = FontFormat.TrueType )
    {
        return new()
        {
            FileName = fileName,
            Format = format,
        };
    }

    /// <summary>
    /// Creates a font source from a URL.
    /// </summary>
    /// <param name="url">Font URL.</param>
    /// <param name="format">Font format.</param>
    /// <returns>A font source.</returns>
    public static FontSource FromUrl( string url, FontFormat format = FontFormat.TrueType )
    {
        return new()
        {
            Url = url,
            Format = format,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// URL used by browser-based rendering.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Raw font bytes used by renderers that need to embed fonts.
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Local file name used by renderers that need to read font bytes.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Font source format.
    /// </summary>
    public FontFormat Format { get; set; } = FontFormat.TrueType;

    #endregion
}