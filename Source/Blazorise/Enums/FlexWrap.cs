namespace Blazorise
{
    /// <summary>
    /// Controls whether the flex container is single-line or multi-line.
    /// </summary>
    public enum FlexWrap
    {
        /// <summary>
        /// No wrap will be applied.
        /// </summary>
        Default,

        /// <summary>
        /// Flex items will wrap onto multiple lines, from top to bottom.
        /// </summary>
        Wrap,

        /// <summary>
        /// Flex items will wrap onto multiple lines from bottom to top.
        /// </summary>
        ReverseWrap,

        /// <summary>
        /// All flex items will be on one line.
        /// </summary>
        NoWrap,
    }
}
