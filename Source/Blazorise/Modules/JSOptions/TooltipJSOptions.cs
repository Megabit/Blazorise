namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a tooltip component.
/// </summary>
public class TooltipJSOptions
{
    /// <summary>
    /// Gets or sets the text content to be displayed within the tooltip.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the placement of the tooltip relative to the target (e.g., "top", "bottom").
    /// </summary>
    public string Placement { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tooltip text can span multiple lines.
    /// </summary>
    public bool Multiline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tooltip is always active and visible.
    /// </summary>
    public bool AlwaysActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an arrow should be displayed pointing to the target.
    /// </summary>
    public bool ShowArrow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tooltip should fade in and out.
    /// </summary>
    public bool Fade { get; set; }

    /// <summary>
    /// Gets or sets the duration of the fade animation, in milliseconds.
    /// </summary>
    public int FadeDuration { get; set; }

    /// <summary>
    /// Gets or sets the trigger event for displaying the tooltip (e.g., "hover", "click").
    /// </summary>
    public string Trigger { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that serves as the target for the tooltip trigger.
    /// </summary>
    public string TriggerTargetId { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the tooltip.
    /// </summary>
    public string MaxWidth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tooltip should automatically detect inline elements.
    /// </summary>
    public bool AutodetectInline { get; set; }

    /// <summary>
    /// Gets or sets the z-index of the tooltip to control its stacking order.
    /// </summary>
    public int? ZIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tooltip can be interacted with (e.g., hovered or clicked).
    /// </summary>
    public bool Interactive { get; set; }

    /// <summary>
    /// Gets or sets the CSS selector or element where the tooltip should be appended.
    /// </summary>
    public string AppendTo { get; set; }

    /// <summary>
    /// Gets or sets the delay for showing and hiding the tooltip.
    /// </summary>
    public TooltipDelay Delay { get; set; }
}

/// <summary>
/// Represents the delay settings for showing and hiding a tooltip.
/// </summary>
public class TooltipDelay
{
    /// <summary>
    /// Gets or sets the delay in milliseconds before the tooltip is shown.
    /// </summary>
    public int Show { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds before the tooltip is hidden.
    /// </summary>
    public int Hide { get; set; }
}