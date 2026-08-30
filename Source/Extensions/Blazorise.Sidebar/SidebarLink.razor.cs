#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar;

/// <summary>
/// Navigates from a sidebar item and controls an attached submenu when present.
/// </summary>
public partial class SidebarLink : BaseComponent
{
    #region Members

    private bool visible;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "sidebar-link" );
        builder.Append( "collapsed", Collapsable && !Visible );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentSidebarItem?.NotifyHasSidebarLink();

        base.OnInitialized();
    }

    /// <summary>
    /// Handles the link click event.
    /// </summary>
    /// <returns></returns>
    protected async Task ClickHandler()
    {
        await Click.InvokeAsync();

        if ( Collapsable )
        {
            Visible = !Visible;

            await InvokeAsync( StateHasChanged );

            await Toggled.InvokeAsync( Visible );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Whether this link controls a nested sidebar item.
    /// </summary>
    protected bool Collapsable => ParentSidebarItem?.HasSubItem == true;

    /// <summary>
    /// Browser data attribute used to activate submenu collapsing.
    /// </summary>
    protected string DataToggle => Collapsable ? "sidebar-collapse" : null;

    /// <summary>
    /// Accessible expansion state exposed for collapsible links.
    /// </summary>
    protected string AriaExpanded => Collapsable ? ( Visible ? "true" : "false" ) : null;

    /// <summary>
    /// Gets the combined list of link attributes and any receiving attribute.
    /// </summary>
    protected Dictionary<string, object> LinkAttributes
    {
        get
        {
            var linkAttributes = new Dictionary<string, object>()
            {
                { "data-toggle", DataToggle },
                { "aria-expanded", AriaExpanded },
            };

            if ( Attributes != null )
                return linkAttributes.Concat( Attributes ).ToDictionary( x => x.Key, x => x.Value );

            return linkAttributes;
        }
    }

    /// <summary>
    /// Specifies the visibility of the link.
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

    /// <summary>
    /// Page address.
    /// </summary>
    [Parameter] public string To { get; set; }

    /// <summary>
    /// The target attribute specifies where to open the linked document.
    /// </summary>
    [Parameter] public Target Target { get; set; } = Target.Default;

    /// <summary>
    /// URL matching behavior for a link.
    /// </summary>
    [Parameter] public Match Match { get; set; } = Match.All;

    /// <summary>
    /// A callback function that is used to compare current uri with the user defined uri. If defined, the <see cref="Match"/> parameter will be ignored.
    /// </summary>
    [Parameter] public Func<string, bool> CustomMatch { get; set; }

    /// <summary>
    /// Specify extra information about the element.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// An event that is raised when the link is clicked.
    /// </summary>
    [Parameter] public EventCallback Click { get; set; }

    /// <summary>
    /// An event that is raised when the link is toggled.
    /// </summary>
    [Parameter] public EventCallback<bool> Toggled { get; set; }

    /// <summary>
    /// Item that supplies this link's submenu relationship.
    /// </summary>
    [CascadingParameter] public SidebarItem ParentSidebarItem { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="SidebarLink"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}