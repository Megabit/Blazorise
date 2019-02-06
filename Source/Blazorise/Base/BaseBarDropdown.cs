#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseBarDropdown : BaseComponent
    {
        #region Members

        private bool isOpen;

        private BaseBarDropdownMenu barDropdownMenu;

        private BaseBarDropdownToggle barDropdownToggler;

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

        internal void Hook( BaseBarDropdownMenu barDropdownMenu )
        {
            this.barDropdownMenu = barDropdownMenu;
        }

        internal void Hook( BaseBarDropdownToggle barDropdownToggler )
        {
            this.barDropdownToggler = barDropdownToggler;
        }

        public void Open()
        {
            var temp = IsOpen;

            IsOpen = true;

            if ( temp != IsOpen ) // used to prevent toggle event call if Open() is called multiple times
                Toggled?.Invoke( IsOpen );

            BarItem?.MenuChanged();

            StateHasChanged();
        }

        public void Close()
        {
            var temp = IsOpen;

            IsOpen = false;

            if ( temp != IsOpen ) // used to prevent toggle event call if Close() is called multiple times
                Toggled?.Invoke( IsOpen );

            BarItem?.MenuChanged();

            StateHasChanged();
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            Toggled?.Invoke( IsOpen );

            BarItem?.MenuChanged();

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

                if ( barDropdownMenu != null )
                    barDropdownMenu.IsOpen = value;

                if ( barDropdownToggler != null )
                    barDropdownToggler.IsOpen = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected Action<bool> Toggled { get; set; }

        [CascadingParameter] protected BaseBarItem BarItem { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
