namespace Blazorise.Components;

/// <summary>
/// Provides configuration options for the <see cref="TransferList{TItem}"/> component.
/// </summary>
public class TransferListOptions
{
    /// <summary>
    /// Option for the default color of the move and "Move All" buttons.
    /// </summary>
    public Color MoveButtonsColor { get; set; } = Color.Primary;

    /// <summary>
    /// Option for the default icon name of the move to end (right) button.
    /// </summary>
    public IconName MoveToEndIconName { get; set; } = IconName.ChevronRight;

    /// <summary>
    /// Option for the default icon name of the move all to end (right) button.
    /// </summary>
    public IconName MoveAllToEndIconName { get; set; } = IconName.ChevronDoubleRight;

    /// <summary>
    /// Option for the default icon name of the move to start (left) button.
    /// </summary>
    public IconName MoveToStartIconName { get; set; } = IconName.ChevronLeft;

    /// <summary>
    /// Option for the default icon name of the move all to start (left) button.
    /// </summary>
    public IconName MoveAllToStartIconName { get; set; } = IconName.ChevronDoubleLeft;

    /// <summary>
    /// Option for the default text color of the move icons.
    /// </summary>
    public TextColor MoveButtonsIconTextColor { get; set; } = TextColor.White;

    /// <summary>
    /// Option for the default style of the move icons.
    /// </summary>
    public IconStyle? MoveButtonsIconStyle { get; set; }

    /// <summary>
    /// Option for the default size of the move icons.
    /// </summary>
    public IconSize? MoveButtonsIconSize { get; set; }
}