namespace Blazorise.QRCode;

/// <summary>
/// Extended Channel Interpretation Identifiers. It is used to tell the barcode reader details about the
/// used references for encoding the data in the symbol. Current implementation consists all well known
/// charset encodings. Currently, it is used only for QR 2D barcode.
/// </summary>
public enum EciMode
{
    /// <summary>
    /// No Extended Channel Interpretation.
    /// </summary>
    Default = 0,

    /// <summary>
    /// ISO/IEC 8859-1 Latin alphabet No. 1 encoding.
    /// </summary>
    Iso8859_1 = 3,

    /// <summary>
    /// ISO/IEC 8859-2 Latin alphabet No. 2 encoding.
    /// </summary>
    Iso8859_2 = 4,

    /// <summary>
    /// ISO/IEC 10646 UTF-8 encoding.
    /// </summary>
    Utf8 = 26
}