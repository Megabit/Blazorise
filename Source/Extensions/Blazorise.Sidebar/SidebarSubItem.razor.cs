#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar;

public partial class SidebarSubItem : BaseComponent
{
    #region Members

    private bool visible;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "sidebar-subitem" );
        builder.Append( "show", Visible );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentSidebarItem?.NotifyHasSidebarSubItem();

        base.OnInitialized();
    }

    /// <summary>
    /// Toggles the visibility of subitem.
    /// </summary>
    /// <param name="visible">Used to override default behaviour.</param>
    public void Toggle( bool? visible = null )
    {
        Visible = visible ?? !Visible;

        InvokeAsync( StateHasChanged );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the visibility of the subitem.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;

            DirtyClasses();
        }
    }

    [CascadingParameter] public SidebarItem ParentSidebarItem { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SidebarSubItem"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}