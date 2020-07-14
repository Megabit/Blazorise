#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
{
    public partial class Sidebar : BaseComponent
    {
        #region Members

        private bool visible;

        private bool scrollable = true;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "sidebar" );
            builder.Append( "sidebar-scrollable", Scrollable );
            builder.Append( "show", Visible );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Opens the sidebar.
        /// </summary>
        public void Open()
        {
            Visible = true;

            StateHasChanged();
        }

        /// <summary>
        /// Closes the sidebar.
        /// </summary>
        public void Close()
        {
            Visible = false;

            StateHasChanged();
        }

        /// <summary>
        /// Toggles the sidebar open or close state.
        /// </summary>
        public void Toggle()
        {
            Visible = !Visible;

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sidebar visibility state.
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
        /// If true, the sidebar will have vertical scrollbar.
        /// </summary>
        [Parameter]
        public bool Scrollable
        {
            get => scrollable;
            set
            {
                scrollable = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Data for generating sidebar dynamically.
        /// </summary>
        [Parameter] public SidebarInfo Data { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
