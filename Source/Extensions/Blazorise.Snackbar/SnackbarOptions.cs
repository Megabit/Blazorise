#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Snackbar;

/// <summary>
/// Holds the snackbar message state.
/// </summary>
public class SnackbarOptions
{
    /// <summary>
    /// Custom key for snack bar message.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Custom snackbar template to render.
    /// </summary>
    public RenderFragment MessageTemplate { get; set; }

    /// <summary>
    /// Flag that indicates if close button will be visible.
    /// </summary>
    public bool ShowCloseButton { get; set; }

    /// <summary>
    /// Custom close button text.
    /// </summary>
    public string CloseButtonText { get; set; }

    /// <summary>
    /// Custom close button icon. Can be either enum <see cref="IconName"/> or string eg. "fa-times".
    /// </summary>
    public object CloseButtonIcon { get; set; }

    /// <summary>
    /// Flag that indicates if action button will be visible.
    /// </summary>
    public bool ShowActionButton { get; set; }

    /// <summary>
    /// Custom action button text.
    /// </summary>
    public string ActionButtonText { get; set; }

    /// <summary>
    /// Custom action button icon. Can be either enum <see cref="IconName"/> or string eg. "fa-times".
    /// </summary>
    public object ActionButtonIcon { get; set; }

    /// <summary>
    /// Time in millisecond for snackbar animation.
    /// </summary>
    public double? AnimationInterval { get; set; }

    /// <summary>
    /// Time in millisecond until snackbar is automatically closed.
    /// </summary>
    public double? IntervalBeforeClose { get; set; }

    /// <summary>
    /// Defines if the snackbar will contain multiple lines.
    /// </summary>
    public bool Multiline { get; set; }
}