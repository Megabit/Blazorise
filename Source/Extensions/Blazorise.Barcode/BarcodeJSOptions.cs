using System.Collections.Generic;

namespace Blazorise.Barcode;

/// <summary>
/// Represents JavaScript options for configuring a barcode generator.
/// </summary>
public class BarcodeJSOptions
{
    /// <summary>
    /// Gets or sets the value or content encoded within the barcode.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the barcode type.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets how the barcode will be rendered.
    /// </summary>
    public string RenderMode { get; set; }

    /// <summary>
    /// Gets or sets the target width of the barcode.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the target height of the barcode.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the scale of the barcode.
    /// </summary>
    public int Scale { get; set; }

    /// <summary>
    /// Gets or sets the foreground color.
    /// </summary>
    public string ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether human-readable text should be shown.
    /// </summary>
    public bool ShowText { get; set; }

    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    public string TextAlignment { get; set; }

    /// <summary>
    /// Gets or sets the barcode rotation.
    /// </summary>
    public string Rotation { get; set; }

    /// <summary>
    /// Gets or sets the top padding.
    /// </summary>
    public int? PaddingTop { get; set; }

    /// <summary>
    /// Gets or sets the right padding.
    /// </summary>
    public int? PaddingRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom padding.
    /// </summary>
    public int? PaddingBottom { get; set; }

    /// <summary>
    /// Gets or sets the left padding.
    /// </summary>
    public int? PaddingLeft { get; set; }

    /// <summary>
    /// Gets or sets additional provider options.
    /// </summary>
    public Dictionary<string, object> ProviderOptions { get; set; }
}