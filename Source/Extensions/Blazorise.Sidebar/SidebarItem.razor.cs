#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar;

public partial class SidebarItem : BaseComponent
{
    #region Members

    private bool hasLink;

    private bool hasSubItem;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "sidebar-item" );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Notifies the parent <see cref="Sidebar"/> that this <see cref="SidebarItem"/> has a link.
    /// </summary>
    internal void NotifyHasSidebarLink()
    {
        hasLink = true;
    }

    /// <summary>
    /// Notifies the parent <see cref="Sidebar"/> that this <see cref="SidebarItem"/> has a sub-item.
    /// </summary>
    internal void NotifyHasSidebarSubItem()
    {
        hasSubItem = true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the value that indicates whether this <see cref="SidebarItem"/> has a link.
    /// </summary>
    public bool HasLink => hasLink;

    /// <summary>
    /// Gets the value that indicates whether this <see cref="SidebarItem"/> has a sub-item.
    /// </summary>
    public bool HasSubItem => hasSubItem;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SidebarItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}