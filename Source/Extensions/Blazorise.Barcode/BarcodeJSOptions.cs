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
    /// Gets or sets the maximum requested barcode symbol width.
    /// </summary>
    public int? MaxSymbolWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum requested barcode symbol height.
    /// </summary>
    public int? MaxSymbolHeight { get; set; }

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
    /// Gets or sets a value indicating whether the encoded value should be shown as human-readable text.
    /// </summary>
    public bool ShowValue { get; set; }

    /// <summary>
    /// Gets or sets the human-readable value alignment.
    /// </summary>
    public string ValueAlignment { get; set; }

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