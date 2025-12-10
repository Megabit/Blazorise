namespace Blazorise;

/// <summary>
/// Options to override notification appearance.
/// </summary>
public class NotificationOptions
{
    /// <summary>
    /// Custom text for the OK button. Shows only if <see cref="ShowOkButton"/> is true.
    /// </summary>
    public string OkButtonText { get; set; }

    /// <summary>
    /// Custom icon for the OK button. Shows only if <see cref="ShowOkButton"/> is true.
    /// </summary>
    public object OkButtonIcon { get; set; }

    /// <summary>
    /// Indicates if the OK button will be visible.
    /// </summary>
    public bool ShowOkButton { get; set; }

    /// <summary>
    /// Creates the default notification options.
    /// </summary>
    /// <returns>Default notification options.</returns>
    public static NotificationOptions Default => new()
    {
        OkButtonIcon = "OK",
        Multiline = true,
    };

    /// <summary>
    /// Delay in milliseconds to wait before closing.
    /// </summary>
    public double? IntervalBeforeClose { get; set; }

    /// <summary>
    /// Defines if the notification will contain multiple lines.
    /// </summary>
    public bool Multiline { get; set; }
}