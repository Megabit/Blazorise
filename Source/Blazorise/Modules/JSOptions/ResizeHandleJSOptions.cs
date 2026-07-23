namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for a <see cref="ResizeHandle"/>.
/// </summary>
public class ResizeHandleJSOptions
{
    /// <summary>
    /// Supplies logical start and end targets for coordinated resizing.
    /// </summary>
    public ResizeHandleTargets Targets { get; set; }

    /// <summary>
    /// Gets or sets the target element ID. When omitted, the handle's parent element is used.
    /// </summary>
    public string TargetElementId { get; set; }

    /// <summary>
    /// Gets or sets whether the separator is vertical.
    /// </summary>
    public bool Vertical { get; set; }

    /// <summary>
    /// Gets or sets whether movement toward the start of the axis increases the size.
    /// </summary>
    public bool ResizeFromStart { get; set; }

    /// <summary>
    /// Gets or sets the CSS property changed while resizing.
    /// </summary>
    public string ResizeProperty { get; set; }

    /// <summary>
    /// Supplies the controlled resize value in pixels.
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Defines the minimum resize value in pixels.
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Defines the maximum resize value in pixels.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets the keyboard resize step in pixels.
    /// </summary>
    public double KeyboardStep { get; set; }

    /// <summary>
    /// Gets or sets the minimum interval in milliseconds between resize notifications.
    /// </summary>
    public int ResizeEventInterval { get; set; }

    /// <summary>
    /// Gets or sets whether resizing is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Defines the provider-resolved class names used for keyboard-visible focus.
    /// </summary>
    public string FocusedClassNames { get; set; }

    /// <summary>
    /// Gets or sets the provider-resolved class names applied to the handle while resizing.
    /// </summary>
    public string ResizingClassNames { get; set; }

    /// <summary>
    /// Gets or sets the provider-resolved class names applied to the target while resizing.
    /// </summary>
    public string TargetResizingClassNames { get; set; }

    /// <summary>
    /// Gets or sets whether resize-start notifications are enabled.
    /// </summary>
    public bool NotifyResizeStarted { get; set; }

    /// <summary>
    /// Gets or sets whether resize progress notifications are enabled.
    /// </summary>
    public bool NotifyResizing { get; set; }

    /// <summary>
    /// Gets or sets whether resize-end notifications are enabled.
    /// </summary>
    public bool NotifyResizeEnded { get; set; }
}