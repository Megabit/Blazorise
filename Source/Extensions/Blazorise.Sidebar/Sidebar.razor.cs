#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Sidebar
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

        /// <summary>
        /// Opens the sidebar.
        /// </summary>
        public void Open()
        {
            IsOpen = true;

            StateHasChanged();
        }

        /// <summary>
        /// Closes the sidebar.
        /// </summary>
        public void Close()
        {
            IsOpen = false;

            StateHasChanged();
        }

        /// <summary>
        /// Toggles the sidebar open or close state.
        /// </summary>
        public void Toggle()
        {
            IsOpen = !IsOpen;

            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sidebar visibility state.
        /// </summary>
        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
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
