/// <summary>
/// Holds the information about the current state of the <see cref="Offcanvas"/> component.
/// </summary>
public record OffcanvasState
{
    /// <summary>
    /// Defines the visibility of modal dialog.
    /// </summary>
    public bool Visible { get; init; }

    /// <summary>
    /// Defines the open state of the modal dialog.
    /// </summary>
    public int OpenIndex { get; init; }
}