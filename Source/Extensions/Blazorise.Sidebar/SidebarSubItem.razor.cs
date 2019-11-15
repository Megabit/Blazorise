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

        private bool isShow;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "sidebar-subitem" );
            builder.Append( "show", IsShow );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Toggles the visibility of subitem.
        /// </summary>
        /// <param name="isShow">Used to override default behaviour.</param>
        public void Toggle( bool? isShow = null )
        {
            IsShow = isShow ?? !IsShow;

            StateHasChanged();
        }

        #endregion

        #region Properties

        [Parameter]
        public bool IsShow
        {
            get => isShow;
            set
            {
                isShow = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
