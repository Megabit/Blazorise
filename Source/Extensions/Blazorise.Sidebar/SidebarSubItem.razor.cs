#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
{
    public abstract class BaseSidebarSubItem : BaseComponent
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
