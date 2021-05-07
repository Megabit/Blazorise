namespace Blazorise
{
    /// <summary>
    /// Screen reader visibility.
    /// </summary>
    public enum Screenreader
    {
        /// <summary>
        /// Default.
        /// </summary>
        Always,

        /// <summary>
        /// Hide an element to all devices except screen readers.
        /// </summary>
        Only,

        /// <summary>
        /// Show the element again when it’s focused.
        /// </summary>
        OnlyFocusable,
    }
}
