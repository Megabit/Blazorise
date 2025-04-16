namespace Blazorise;

/// <summary>
/// Defines the possible UI message types with predefined actions.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Shows the simple info message.
    /// </summary>
    Info,

    /// <summary>
    /// Shows the success message.
    /// </summary>
    Success,

    /// <summary>
    /// Shows the warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Shows the error message.
    /// </summary>
    Error,

    /// <summary>
    /// Prompt the user with the confirmation dialog.
    /// </summary>
    Confirmation,

    /// <summary>
    /// Represents a selection option within a set of choices.
    /// </summary>
    Choice,
}