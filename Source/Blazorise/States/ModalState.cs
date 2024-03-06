namespace Blazorise.States;

/// <summary>
/// Represents the state of a modal dialog, including visibility and animation states.
/// </summary>
public record ModalState
{
    /// <summary>
    /// Gets a value indicating whether the modal is currently visible.
    /// </summary>
    /// <value><c>true</c> if the modal is visible; otherwise, <c>false</c>.</value>
    public bool Visible { get; init; }

    /// <summary>
    /// Gets a value indicating whether the modal is currently being shown (animating into view).
    /// </summary>
    /// <value><c>true</c> if the modal is showing; otherwise, <c>false</c>.</value>
    public bool Showing { get; init; }

    /// <summary>
    /// Gets a value indicating whether the modal is currently being hidden (animating out of view).
    /// </summary>
    /// <value><c>true</c> if the modal is hiding; otherwise, <c>false</c>.</value>
    public bool Hiding { get; init; }

    /// <summary>
    /// Gets the index indicating the order in which this modal was opened relative to other modals.
    /// </summary>
    /// <value>The open index of the modal.</value>
    public int OpenIndex { get; init; }
}