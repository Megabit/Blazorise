namespace Blazorise.Barcode;

/// <summary>
/// Defines the barcode symbology.
/// </summary>
public enum BarcodeType
{
    /// <summary>
    /// Two-dimensional matrix symbology commonly used for URLs, text, and other compact payloads that are scanned by mobile devices.
    /// </summary>
    QrCode,

    /// <summary>
    /// Compact two-dimensional matrix symbology for small item marking, industrial labels, and dense alphanumeric data.
    /// </summary>
    DataMatrix,

    /// <summary>
    /// Two-dimensional matrix symbology that does not require a quiet zone, often used for transport tickets and mobile passes.
    /// </summary>
    Aztec,

    /// <summary>
    /// Stacked linear symbology based on Code 128 for encoding longer alphanumeric messages in a compact rectangular symbol.
    /// </summary>
    CodablockF,

    /// <summary>
    /// Fixed-size two-dimensional matrix symbology optimized for parcel and logistics sorting labels.
    /// </summary>
    MaxiCode,

    /// <summary>
    /// Reduced-size stacked variant of PDF417 for constrained labels that need more capacity than a linear barcode.
    /// </summary>
    MicroPdf417,

    /// <summary>
    /// Stacked linear symbology for larger text or data payloads, commonly used on identification and transport documents.
    /// </summary>
    Pdf417,

    /// <summary>
    /// Two-dimensional matrix symbology designed for Chinese characters and other mixed data content.
    /// </summary>
    HanXin,

    /// <summary>
    /// Two-dimensional dot-based symbology suited to high-speed printing and item-level tracking.
    /// </summary>
    DotCode,

    /// <summary>
    /// Royal Mail postal symbology used for UK mail sorting and tracking data.
    /// </summary>
    Mailmark,

    /// <summary>
    /// High-density linear symbology for alphanumeric data, commonly used for shipping and inventory labels.
    /// </summary>
    Code128,

    /// <summary>
    /// Linear alphanumeric symbology for uppercase letters, digits, and a limited symbol set, commonly used in industrial labels.
    /// </summary>
    Code39,

    /// <summary>
    /// Compact linear alphanumeric symbology with higher density and stronger checks than Code 39.
    /// </summary>
    Code93,

    /// <summary>
    /// Linear numeric symbology with start and stop characters, commonly used by libraries, blood banks, and legacy tracking systems.
    /// </summary>
    Codabar,

    /// <summary>
    /// Thirteen-digit retail product symbology for global trade item numbers, commonly used at point of sale.
    /// </summary>
    Ean13,

    /// <summary>
    /// Eight-digit compact retail product symbology for small packages where EAN-13 is too large.
    /// </summary>
    Ean8,

    /// <summary>
    /// Numeric-only high-density linear symbology that encodes digit pairs, often used for warehouse and distribution labels.
    /// </summary>
    Interleaved2Of5,

    /// <summary>
    /// Fourteen-digit Interleaved 2 of 5 symbology for GTIN-14 shipping container and carton identifiers.
    /// </summary>
    Itf14,

    /// <summary>
    /// Numeric-only linear symbology derived from Modified Plessey, commonly used for inventory and shelf labels.
    /// </summary>
    Msi,

    /// <summary>
    /// Numeric-only linear symbology used in pharmaceutical packaging and distribution.
    /// </summary>
    Pharmacode,

    /// <summary>
    /// Twelve-digit retail product symbology widely used for point-of-sale scanning in North America.
    /// </summary>
    UpcA,

    /// <summary>
    /// Zero-compressed UPC symbology for small retail packages where UPC-A is too large.
    /// </summary>
    UpcE
}