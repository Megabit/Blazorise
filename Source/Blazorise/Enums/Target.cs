#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// The target attribute specifies where to open the linked document.
    /// </summary>
    public enum Target
    {
        /// <summary>
        /// No target will be applied. Usually this is the same as <see cref="Target.Self"/>.
        /// </summary>
        None,

        /// <summary>
        /// Opens the linked document in the same frame as it was clicked (this is default).
        /// </summary>
        Self,

        /// <summary>
        /// Opens the linked document in a new window or tab.
        /// </summary>
        Blank,

        /// <summary>
        /// Opens the linked document in the parent frame.
        /// </summary>
        Parent,

        /// <summary>
        /// Opens the linked document in the full body of the window.
        /// </summary>
        Top,
    }
}
