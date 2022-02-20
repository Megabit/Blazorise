namespace Blazorise
{
    /// <summary>
    /// Defines the alignment along the main axis.
    /// </summary>
    public enum FlexJustifyContent
    {
        /// <summary>
        /// Justify-content will not be applied.
        /// </summary>
        Default,

        /// <summary>
        /// Items are packed toward the start of the flex-direction.
        /// </summary>
        Start,

        /// <summary>
        /// Items are packed toward the end of the flex-direction.
        /// </summary>
        End,

        /// <summary>
        /// Items are centered along the line.
        /// </summary>
        Center,

        /// <summary>
        /// Items are evenly distributed in the line; first item is on the start line, last item on the end line.
        /// </summary>
        Between,

        /// <summary>
        /// Items are evenly distributed in the line with equal space around them.
        /// Note that visually the spaces aren't equal, since all the items have equal space on both sides.
        /// </summary>
        /// <remarks>
        /// The first item will have one unit of space against the container edge, but two units of space
        /// between the next item because that next item has its own spacing that applies.
        /// </remarks>
        Around,
    }
}
