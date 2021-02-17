#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).
    /// </summary>
    public enum JustifyContent
    {
        /// <summary>
        /// Sets this property to its default value.
        /// </summary>
        None,

        /// <summary>
        /// Items are positioned at the beginning of the container.
        /// </summary>
        Start,

        /// <summary>
        /// Items are positioned at the end of the container.
        /// </summary>
        End,

        /// <summary>
        /// Items are positioned at the center of the container.
        /// </summary>
        Center,

        /// <summary>
        /// Items are positioned with space between the lines.
        /// </summary>
        Between,

        /// <summary>
        /// Items are positioned with space before, between, and after the lines.
        /// </summary>
        Around,
    }
}
