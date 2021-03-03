#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Direction of an dropdown menu.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Same as <see cref="Down"/>.
        /// </summary>
        None,

        /// <summary>
        /// Trigger dropdown menus bellow an element (default behaviour).
        /// </summary>
        Down,

        /// <summary>
        /// Trigger dropdown menus above an element.
        /// </summary>
        Up,

        /// <summary>
        /// Trigger dropdown menus to the right of an element.
        /// </summary>
        Right,

        /// <summary>
        /// Trigger dropdown menus to the left of an element.
        /// </summary>
        Left,
    }
}
