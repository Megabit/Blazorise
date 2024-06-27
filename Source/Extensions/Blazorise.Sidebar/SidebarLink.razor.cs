#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar;

public partial class SidebarLink : BaseComponent
{
    #region Members

    private bool visible;

    #endregion

    #region Methods

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "sidebar-link" );
        builder.Append( "collapsed", Collapsable && !Visible );

        base.BuildClasses( builder );
    }

    protected override void OnInitialized()
    {
        ParentSidebarItem?.NotifyHasSidebarLink();

        base.OnInitialized();
    }

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

    protected bool Collapsable => ParentSidebarItem?.HasSubItem == true;

    protected string DataToggle => Collapsable ? "sidebar-collapse" : null;

    protected string AriaExpanded => Collapsable ? Visible.ToString().ToLowerInvariant() : null;

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
    /// A callback function that is used to compare current uri with the user defined uri. Must enable <see cref="Match.Custom"/> to be used.
    /// </summary>
    [Parameter] public Func<string, bool> CustomMatch { get; set; }

    /// <summary>
    /// Specify extra information about the element.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Occurs when the item is clicked.
    /// </summary>
    [Parameter] public EventCallback Click { get; set; }

    [Parameter] public EventCallback<bool> Toggled { get; set; }

    [CascadingParameter] public SidebarItem ParentSidebarItem { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}