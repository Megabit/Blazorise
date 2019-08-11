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

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( "sidebar-subitem" )
                .If( () => "show", () => IsShow );

            base.RegisterClasses();
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
        protected bool IsShow
        {
            get => isShow;
            set
            {
                isShow = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
