using Blazorise.Modules;

namespace Blazorise.Cropper;

/// <summary>
/// Represents JavaScript options for initializing an image cropper component.
/// </summary>
public class CropperInitializeJSOptions
{
    /// <summary>
    /// Gets or sets the source URL of the image to be cropped.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the alternate text for the image.
    /// </summary>
    public string Alt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cropper is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show a background around the cropping area.
    /// </summary>
    public bool ShowBackground { get; set; }

    /// <summary>
    /// Gets or sets options related to the image settings for the cropper.
    /// </summary>
    public CropperImageOptions Image { get; set; }

    /// <summary>
    /// Gets or sets options for the cropping selection area.
    /// </summary>
    public CropperSelectionJSOptions Selection { get; set; }

    /// <summary>
    /// Gets or sets options for the grid overlay in the cropper.
    /// </summary>
    public CropperGridOptions Grid { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of an image cropper component dynamically.
/// </summary>
public class CropperUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the source URL of the image.
    /// </summary>
    public JSOptionChange<string> Source { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the alternate text for the image.
    /// </summary>
    public JSOptionChange<string> Alt { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the cross-origin attribute of the image.
    /// </summary>
    public JSOptionChange<string> CrossOrigin { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the image settings in the cropper.
    /// </summary>
    public JSOptionChange<CropperImageOptions> Image { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the selection area in the cropper.
    /// </summary>
    public JSOptionChange<CropperSelectionJSOptions> Selection { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the grid overlay settings in the cropper.
    /// </summary>
    public JSOptionChange<CropperGridOptions> Grid { get; set; }

    /// <summary>
    /// Gets or sets the option for enabling or disabling the cropper.
    /// </summary>
    public JSOptionChange<bool> Enabled { get; set; }
}

/// <summary>
/// Represents JavaScript options for configuring the selection area within an image cropper component.
/// </summary>
public class CropperSelectionJSOptions
{
    /// <summary>
    /// Gets or sets the aspect ratio for the selection area. A value of <c>null</c> allows any aspect ratio.
    /// </summary>
    public double? AspectRatio { get; set; }

    /// <summary>
    /// Gets or sets the initial aspect ratio for the selection area.
    /// </summary>
    public double? InitialAspectRatio { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection should initially cover the entire image.
    /// </summary>
    public bool? InitialCoverage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection area is movable within the cropper.
    /// </summary>
    public bool Movable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection area is resizable.
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether zooming is allowed on the selection area.
    /// </summary>
    public bool Zoomable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether keyboard navigation is enabled within the cropper.
    /// </summary>
    public bool Keyboard { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection area has an outline displayed.
    /// </summary>
    public bool Outlined { get; set; }
}