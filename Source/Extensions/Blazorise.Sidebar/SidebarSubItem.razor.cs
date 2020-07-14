#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
{
    public partial class SidebarSubItem : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "sidebar-subitem" );
            builder.Append( "show", Visible );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentSidebarItem != null )
            {
                ParentSidebarItem.NotifyHasSidebarSubItem();
            }

            base.OnInitialized();
        }

        /// <summary>
        /// Toggles the visibility of subitem.
        /// </summary>
        /// <param name="visible">Used to override default behaviour.</param>
        public void Toggle( bool? visible = null )
        {
            Visible = visible ?? !Visible;

            StateHasChanged();
        }

        #endregion

        #region Properties

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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
