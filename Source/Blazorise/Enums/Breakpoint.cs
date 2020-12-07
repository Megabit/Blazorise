#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the media breakpoint.
    /// </summary>
    public enum Breakpoint
    {
        /// <summary>
        /// Breakpoint is undefined.
        /// </summary>
        None,

        /// <summary>
        /// Valid on all devices. (extra small)
        /// </summary>
        Mobile,

        /// <summary>
        /// Breakpoint on tablets (small).
        /// </summary>
        Tablet,

        /// <summary>
        ///  Breakpoint on desktop (medium).
        /// </summary>
        Desktop,

        /// <summary>
        /// Breakpoint on widescreen (large).
        /// </summary>
        Widescreen,

        /// <summary>
        /// Breakpoint on large desktops (extra large).
        /// </summary>
        FullHD,
    }
}
