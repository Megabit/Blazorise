namespace Blazorise.Modules;

/// <summary>
/// Provider styling used by the shared panel height animation module.
/// </summary>
public class CollapseJSOptions
{
    /// <summary>
    /// Gets or sets the class applied to the panel when it is not animating.
    /// </summary>
    public string BaseClassName { get; set; }

    /// <summary>
    /// Gets or sets the class identifying an expanded panel.
    /// </summary>
    public string VisibleClassName { get; set; }

    /// <summary>
    /// Gets or sets the class identifying a hidden panel, for providers without a visible class.
    /// </summary>
    public string HiddenClassName { get; set; }

    /// <summary>
    /// Gets or sets the class supplying height transition styles.
    /// </summary>
    public string AnimatingClassName { get; set; }

    /// <summary>
    /// Gets or sets the selector of the ancestor owning animation settings. Null uses the immediate parent.
    /// </summary>
    public string ContainerSelector { get; set; }
}