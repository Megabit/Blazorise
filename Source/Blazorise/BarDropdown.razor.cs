#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseBarDropdown : BaseComponent
    {
        #region Members

        private bool isOpen;

        private BaseBarDropdownMenu barDropdownMenu;

        private BaseBarDropdownToggle barDropdownToggler;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdown() );
            builder.Append( ClassProvider.BarDropdownShow(), IsOpen );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            BarItem?.Hook( this );

            base.OnInitialized();
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
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                if ( barDropdownMenu != null )
                    barDropdownMenu.IsOpen = value;

                if ( barDropdownToggler != null )
                    barDropdownToggler.IsOpen = value;

                DirtyClasses();
            }
        }

        [Parameter] public Action<bool> Toggled { get; set; }

        [CascadingParameter] protected BaseBarItem BarItem { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
