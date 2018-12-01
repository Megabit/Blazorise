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
    public abstract class BaseSidebarLink : BaseComponent
    {
        #region Members

        private bool isShow;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( "sidebar-link" )
                .If( () => "collapsed", () => !IsShow );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Click?.Invoke();

            if ( To == null )
            {
                IsShow = !IsShow;

                StateHasChanged();

                Toggled?.Invoke( IsShow );
            }
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

        [Parameter] protected string To { get; set; }

        [Parameter] protected Match Match { get; set; } = Match.All;

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] protected Action Click { get; set; }

        [Parameter] protected Action<bool> Toggled { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
