﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
{
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
            if ( ParentSidebarItem != null )
            {
                ParentSidebarItem.NotifyHasSidebarLink();
            }

            base.OnInitialized();
        }

        protected async Task ClickHandler()
        {
            await Click.InvokeAsync( null );

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
        [Parameter] public Target Target { get; set; } = Target.None;

        /// <summary>
        /// URL matching behavior for a link.
        /// </summary>
        [Parameter] public Match Match { get; set; } = Match.All;

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
}
