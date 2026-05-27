using Blazorise.Modules;

namespace Blazorise.Cropper;

/// <summary>
/// Represents JavaScript options for initializing an image cropper component.
/// </summary>
public class CropperJSOptions
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
    /// Gets or sets whether cropper interactions are enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets whether the cropper shows a background around the cropping area.
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
    /// Gets or sets how much of the image the initial selection covers.
    /// </summary>
    public double? InitialCoverage { get; set; }

    /// <summary>
    /// Gets or sets whether dragging can reposition the selection area.
    /// </summary>
    public bool Movable { get; set; }

    /// <summary>
    /// Gets or sets whether handles can resize the selection area.
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// Gets or sets whether wheel or gesture zoom can be applied from the selection area.
    /// </summary>
    public bool Zoomable { get; set; }

    /// <summary>
    /// Gets or sets whether keyboard input can move or resize the active selection.
    /// </summary>
    public bool Keyboard { get; set; }

    /// <summary>
    /// Gets or sets whether the selection area renders with a visible outline.
    /// </summary>
    public bool Outlined { get; set; }
}