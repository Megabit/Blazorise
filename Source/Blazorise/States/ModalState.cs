namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="Modal"/> component.
/// </summary>
public record ModalState
{
    /// <summary>
    /// Defines the visibility of modal dialog.
    /// </summary>
    public bool Visible { get; init; }

    public bool Showing { get; init; }

    public bool Hiding { get; init; }

    /// <summary>
    /// Defines the open state of the modal dialog.
    /// </summary>
    public int OpenIndex { get; init; }
}