namespace Blazorise;

/// <summary>
/// Defines the intent of a toast message.
/// </summary>
public enum ToastIntent
{
    /// <summary>
    /// Shows as a regular message.
    /// </summary>
    Default,

    /// <summary>
    /// Shows as an info message.
    /// </summary>
    Info,

    /// <summary>
    /// Shows as a success message.
    /// </summary>
    Success,

    /// <summary>
    /// Shows as a warning message.
    /// </summary>
    Warning,

    /// <summary>
    /// Shows as an error message.
    /// </summary>
    Error,
}