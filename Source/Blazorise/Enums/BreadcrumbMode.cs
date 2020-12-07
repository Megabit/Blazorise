#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the breadcrumb activation mode.
    /// </summary>
    public enum BreadcrumbMode
    {
        /// <summary>
        /// No activation will be applied, meaning it must be applied manually by setting the <see cref="BreadcrumbItem.Active"/> property.
        /// </summary>
        None,

        /// <summary>
        /// Breadcrumb items will be activated based on current navigation.
        /// </summary>
        Auto,
    }
}
