namespace Blazorise
{
    /// <summary>
    /// The overflow property controls what happens to content that is too big to fit into an area.
    /// </summary>
    public enum OverflowType
    {
        /// <summary>
        /// No overflow will be applied
        /// </summary>
        Default,

        /// <summary>
        /// The overflow is not clipped. The content renders outside the element's box.
        /// </summary>
        Visible,

        /// <summary>
        /// The overflow is clipped, and the rest of the content will be invisible.
        /// </summary>
        Hidden,

        /// <summary>
        /// The overflow is clipped, and a scrollbar is added to see the rest of the content.
        /// </summary>
        Scroll,

        /// <summary>
        /// Similar to scroll, but it adds scrollbars only when necessary.
        /// </summary>
        Auto,
    }
}
