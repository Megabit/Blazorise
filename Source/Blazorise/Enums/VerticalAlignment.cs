namespace Blazorise
{
    /// <summary>
    /// Used to change the vertical alignment of inline, inline-block, inline-table, and table cell elements.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Alignment will not be set.
        /// </summary>
        Default,

        /// <summary>
        /// Aligns the baseline of the element with the baseline of its parent.
        /// </summary>
        Baseline,

        /// <summary>
        /// Aligns the top of the element and its descendants with the top of the entire line.
        /// </summary>
        Top,

        /// <summary>
        /// Centers the padding box of the cell within the row.
        /// </summary>
        Middle,

        /// <summary>
        /// Aligns the bottom of the element and its descendants with the bottom of the entire line.
        /// </summary>
        Bottom,

        /// <summary>
        /// Aligns the top of the element with the top of the parent element's font.
        /// </summary>
        TextTop,

        /// <summary>
        /// Aligns the bottom of the element with the bottom of the parent element's font.
        /// </summary>
        TextBottom,
    }
}
