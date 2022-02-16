using System;

namespace Blazorise
{
    /// <summary>
    /// Defines the side on which to apply the spacing.
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// No side.
        /// </summary>
        None,

        /// <summary>
        /// Top side.
        /// </summary>
        Top,

        /// <summary>
        /// Bottom side.
        /// </summary>
        Bottom,

        /// <summary>
        /// Start side.
        /// </summary>
        Start,

        /// <summary>
        /// End side.
        /// </summary>
        End,

        /// <summary>
        /// Left and right side.
        /// </summary>
        X,

        /// <summary>
        /// Top and bottom side.
        /// </summary>
        Y,

        /// <summary>
        /// All 4 sides of the element.
        /// </summary>
        All,
    }
}
