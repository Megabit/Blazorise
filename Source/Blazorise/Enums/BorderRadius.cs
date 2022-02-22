namespace Blazorise
{
    /// <summary>
    /// Radius styles of an element.
    /// </summary>
    public enum BorderRadius
    {
        /// <summary>
        /// No particular rule will be applied, meaning a default borders will be used.
        /// </summary>
        Default,

        /// <summary>
        /// Makes the element rounded on all sides.
        /// </summary>
        Rounded,

        /// <summary>
        /// Makes the element rounded on top side of the element.
        /// </summary>
        RoundedTop,

        /// <summary>
        /// Makes the element rounded on right side of the element.
        /// </summary>
        RoundedEnd,

        /// <summary>
        /// Makes the element rounded on bottom side of the element.
        /// </summary>
        RoundedBottom,

        /// <summary>
        /// Makes the element rounded on left side of the element.
        /// </summary>
        RoundedStart,

        /// <summary>
        /// Makes the element as circle shaped.
        /// </summary>
        RoundedCircle,

        /// <summary>
        /// Makes the element as pill shaped.
        /// </summary>
        RoundedPill,

        /// <summary>
        /// Makes the element without any round corners.
        /// </summary>
        RoundedZero,
    }
}
