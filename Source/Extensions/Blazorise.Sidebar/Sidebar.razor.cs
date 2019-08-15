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
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
