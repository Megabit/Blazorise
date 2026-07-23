using System;

namespace Blazorise;

/// <summary>
/// Supplies information about a resize-handle interaction.
/// </summary>
public class ResizeHandleEventArgs : EventArgs
{
    /// <summary>
    /// Reports the current logical start-target size in pixels.
    /// </summary>
    public double StartSize { get; set; }

    /// <summary>
    /// Reports the logical start-target size captured when the interaction began.
    /// </summary>
    public double PreviousStartSize { get; set; }

    /// <summary>
    /// Reports the current logical end-target size when coordinated resizing is active.
    /// </summary>
    public double? EndSize { get; set; }

    /// <summary>
    /// Reports the logical end-target size captured when coordinated resizing began.
    /// </summary>
    public double? PreviousEndSize { get; set; }

    /// <summary>
    /// Gets or sets the current size in pixels.
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Gets or sets the size in pixels at the start of the interaction.
    /// </summary>
    public double PreviousSize { get; set; }

    /// <summary>
    /// Gets or sets the size difference in pixels from the start of the interaction.
    /// </summary>
    public double Delta { get; set; }

    /// <summary>
    /// Gets or sets the pointer type reported by the browser.
    /// </summary>
    public string PointerType { get; set; }

    /// <summary>
    /// Gets or sets whether the interaction was canceled.
    /// </summary>
    public bool Canceled { get; set; }
}