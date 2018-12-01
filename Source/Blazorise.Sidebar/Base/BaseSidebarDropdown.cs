#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Sidebar.Base
{
    public abstract class BaseSidebarDropdown : BaseComponent
    {
        #region Members

        private bool isShow;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( "sidebar-dropdown" )
                .Add( "sidebar-collapse" )
                .If( () => "show", () => IsShow );

            base.RegisterClasses();
        }

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
