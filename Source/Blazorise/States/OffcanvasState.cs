namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="Offcanvas"/> component.
/// </summary>
public record OffcanvasState
{
    /// <summary>
    /// Defines the visibility of the offcanvas.
    /// </summary>
    public bool Visible { get; init; }
}