namespace Blazorise.QRCode;

/// <summary>
/// Represents JavaScript options for configuring a QR code generator.
/// </summary>
public class QRCodeJSOptions
{
    /// <summary>
    /// Gets or sets the value or content encoded within the QR code.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the error correction level of the QR code (e.g., "L", "M", "Q", "H").
    /// </summary>
    public string EccLevel { get; set; }

    /// <summary>
    /// Gets or sets the color for the dark modules of the QR code.
    /// </summary>
    public string DarkColor { get; set; }

    /// <summary>
    /// Gets or sets the color for the light modules of the QR code.
    /// </summary>
    public string LightColor { get; set; }

    /// <summary>
    /// Gets or sets the number of pixels per module, determining the size of the QR code.
    /// </summary>
    public int PixelsPerModule { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether quiet zones (margins) should be drawn around the QR code.
    /// </summary>
    public bool DrawQuietZones { get; set; }

    /// <summary>
    /// Gets or sets the path or URL of an icon to be displayed in the center of the QR code.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon as a percentage of the QR code size.
    /// </summary>
    public int IconSizePercentage { get; set; }

    /// <summary>
    /// Gets or sets the width of the border around the icon, in pixels.
    /// </summary>
    public int IconBorderWidth { get; set; }
}