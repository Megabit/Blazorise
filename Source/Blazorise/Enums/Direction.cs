using System;

namespace Blazorise
{
    /// <summary>
    /// Direction of an dropdown menu.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// A default direction will be used, in most cases it is the same as <see cref="Down"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Trigger dropdown menus bellow an element (default behaviour).
        /// </summary>
        Down,

        /// <summary>
        /// Trigger dropdown menus above an element.
        /// </summary>
        Up,

        /// <summary>
        /// Trigger dropdown menus to the end of an element.
        /// </summary>
        End,

        /// <summary>
        /// Trigger dropdown menus to the start of an element.
        /// </summary>
        Start,
    }
}
