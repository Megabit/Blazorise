namespace Blazorise
{
    /// <summary>
    /// Specifies the reason that a component was closed.
    /// </summary>
    public enum CloseReason
    {
        /// <summary>
        /// The cause of the closure was not defined or could not be determined.
        /// </summary>
        None,

        /// <summary>
        /// The user is closing the component through the user interface.
        /// </summary>
        UserClosing,

        /// <summary>
        /// The component has lost focus or user has gone out of bounds.
        /// </summary>
        FocusLostClosing,

        /// <summary>
        /// Pressing the escape key is closing the component.
        /// </summary>
        EscapeClosing,
    }
}
