namespace Blazorise
{
    /// <summary>
    /// Type of positions allowed for the fluent position builder.
    /// </summary>
    public enum PositionType
    {
        /// <summary>
        /// Position will not be applied to an element.
        /// </summary>
        Default,

        /// <summary>
        /// An element is not positioned in any special way; it is always positioned according to the normal flow of the page.
        /// </summary>
        Static,

        /// <summary>
        /// An element is positioned relative to its normal position.
        /// </summary>
        Relative,

        /// <summary>
        /// An element is positioned relative to the nearest positioned ancestor (instead of positioned relative to the
        /// viewport, like fixed).
        /// </summary>
        Absolute,

        /// <summary>
        /// An element is positioned relative to the viewport, which means it always stays in the
        /// same place even if the page is scrolled. The top, right, bottom, and left properties are used to position the element.
        /// </summary>
        Fixed,

        /// <summary>
        /// An element is positioned based on the user's scroll position.
        /// </summary>
        Sticky
    }
}
