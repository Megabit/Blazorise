namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="Toast"/> component.
/// </summary>
public record ToastState
{
    /// <summary>
    /// Defines the visibility of the toast.
    /// </summary>
    public bool Visible { get; init; }

    /// <summary>
    /// Flag that indicates that the showing animation is currently active.
    /// </summary>
    public bool Showing { get; init; }

    /// <summary>
    /// Flag that indicates that the hiding animation is currently active.
    /// </summary>
    public bool Hiding { get; init; }
}