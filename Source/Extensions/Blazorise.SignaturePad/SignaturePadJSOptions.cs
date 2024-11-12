using Blazorise.Modules;

namespace Blazorise.SignaturePad;

/// <summary>
/// Represents JavaScript options for initializing a signature pad component.
/// </summary>
public class SignaturePadInitializeJSOptions
{
    /// <summary>
    /// Gets or sets the initial data URL for the signature image.
    /// </summary>
    public string DataUrl { get; set; }

    /// <summary>
    /// Gets or sets the default dot size for points drawn on the signature pad.
    /// </summary>
    public double DotSize { get; set; }

    /// <summary>
    /// Gets or sets the minimum width for lines drawn on the signature pad.
    /// </summary>
    public double MinLineWidth { get; set; }

    /// <summary>
    /// Gets or sets the maximum width for lines drawn on the signature pad.
    /// </summary>
    public double MaxLineWidth { get; set; }

    /// <summary>
    /// Gets or sets the throttling interval, in milliseconds, for limiting data points.
    /// </summary>
    public int Throttle { get; set; }

    /// <summary>
    /// Gets or sets the minimum distance between points to be considered as separate.
    /// </summary>
    public int MinDistance { get; set; }

    /// <summary>
    /// Gets or sets the background color of the signature pad.
    /// </summary>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the color of the pen used for drawing on the signature pad.
    /// </summary>
    public string PenColor { get; set; }

    /// <summary>
    /// Gets or sets the velocity filter weight for line thickness adjustment based on speed.
    /// </summary>
    public double VelocityFilterWeight { get; set; }

    /// <summary>
    /// Gets or sets the image type for exporting the signature (e.g., "image/png").
    /// </summary>
    public string ImageType { get; set; }

    /// <summary>
    /// Gets or sets the quality of the exported image, from 0 to 1, if applicable.
    /// </summary>
    public double? ImageQuality { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include the background color when exporting the image.
    /// </summary>
    public bool IncludeImageBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the signature pad is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a signature pad component dynamically.
/// </summary>
public class SignaturePadUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the data URL of the signature image.
    /// </summary>
    public JSOptionChange<string> DataUrl { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the dot size on the signature pad.
    /// </summary>
    public JSOptionChange<double> DotSize { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the minimum line width.
    /// </summary>
    public JSOptionChange<double> MinLineWidth { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the maximum line width.
    /// </summary>
    public JSOptionChange<double> MaxLineWidth { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the throttling interval in milliseconds.
    /// </summary>
    public JSOptionChange<int> Throttle { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the minimum distance between points.
    /// </summary>
    public JSOptionChange<int> MinDistance { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the background color of the signature pad.
    /// </summary>
    public JSOptionChange<string> BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the pen color used for drawing.
    /// </summary>
    public JSOptionChange<string> PenColor { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the velocity filter weight for line thickness.
    /// </summary>
    public JSOptionChange<double> VelocityFilterWeight { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the image type used for exporting.
    /// </summary>
    public JSOptionChange<string> ImageType { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the image quality when exporting.
    /// </summary>
    public JSOptionChange<double?> ImageQuality { get; set; }

    /// <summary>
    /// Gets or sets the option for including the background color in the exported image.
    /// </summary>
    public JSOptionChange<bool> IncludeImageBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the option for setting the signature pad as read-only.
    /// </summary>
    public JSOptionChange<bool> ReadOnly { get; set; }
}