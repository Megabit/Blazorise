#region Using directives
using System;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Contains resource data resolved for PDF generation.
/// </summary>
public sealed class PdfResourceContent
{
    #region Constructors

    /// <summary>
    /// Initializes resolved PDF resource content.
    /// </summary>
    /// <param name="data">Resource bytes.</param>
    /// <param name="mediaType">Resource media type.</param>
    public PdfResourceContent( byte[] data, string mediaType = null )
    {
        Data = data ?? throw new ArgumentNullException( nameof( data ) );
        MediaType = mediaType;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Resource bytes.
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// Resource media type when known.
    /// </summary>
    public string MediaType { get; }

    #endregion
}