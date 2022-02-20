namespace Blazorise
{
    /// <summary>
    /// Determines how the text will behave when it is larger than a parent container.
    /// </summary>
    public enum TextOverflow
    {
        /// <summary>
        /// No overflow will be applied.
        /// </summary>
        Default,

        /// <summary>
        /// Text will wrap into a new line when it reaches the end of container.
        /// </summary>
        Wrap,

        /// <summary>
        /// Prevents text from wrapping.
        /// </summary>
        NoWrap,

        /// <summary>
        /// Truncate the text with an ellipsis.
        /// </summary>
        Truncate,
    }
}
