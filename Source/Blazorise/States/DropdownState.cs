namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="Dropdown"/> component.
/// </summary>
public record DropdownState
{
    #region Properties

    /// <summary>
    /// If true, a dropdown menu will be visible.
    /// </summary>
    public bool Visible { get; init; }

    /// <summary>
    /// If true, a dropdown menu will be right aligned.
    /// </summary>
    public bool EndAligned { get; init; }

    /// <summary>
    /// If true, dropdown would not react to button click.
    /// </summary>
    public bool Disabled { get; init; }

    /// <summary>
    /// Dropdown-menu slide direction.
    /// </summary>
    public Direction Direction { get; init; }

    #endregion
}