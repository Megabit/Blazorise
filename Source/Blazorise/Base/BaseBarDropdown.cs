#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseBarDropdown : BaseComponent
    {
        #region Members

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BarDropdown() )
                .If( () => ClassProvider.BarDropdownShow(), () => IsOpen );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            BarItem?.Hook( this );

            base.OnInit();
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            Toggled?.Invoke( IsOpen );

            StateHasChanged();
        }

        #endregion

        #region Properties

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

        [Parameter] protected Action<bool> Toggled { get; set; }

        [CascadingParameter] protected BarItem BarItem { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
