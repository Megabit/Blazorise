namespace Blazorise
{
    /// <summary>
    /// Defines the possible ui message types with predefined actions.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Shows the simple info message.
        /// </summary>
        Info,

        /// <summary>
        /// Shows the sucess message.
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
    }
}
