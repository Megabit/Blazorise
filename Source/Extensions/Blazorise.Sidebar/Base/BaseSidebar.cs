#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar.Base
{
    public abstract class BaseSidebar : BaseComponent
    {
        #region Members

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( "sidebar" )
                .If( "show", () => IsOpen );

            base.RegisterClasses();
        }

        public void Open()
        {
            IsOpen = true;

            StateHasChanged();
        }

        public void Close()
        {
            IsOpen = false;

            StateHasChanged();
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Handles the visibility of sidebar.
        /// </summary>
        [Parameter]
        internal bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
