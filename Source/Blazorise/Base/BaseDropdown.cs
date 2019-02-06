#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseDropdown : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isRightAligned;

        private Direction direction = Blazorise.Direction.Down;

        private BaseDropdownMenu dropdownMenu;

        private BaseDropdownToggle dropdownToggle;

        private List<BaseButton> registeredButtons;

        #endregion

        #region Methods

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( registeredButtons != null )
                {
                    registeredButtons.Clear();
                    registeredButtons = null;
                }

                dropdownMenu = null;
                dropdownToggle = null;
            }

            base.Dispose( disposing );
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Dropdown() )
                .If( () => ClassProvider.DropdownGroup(), () => IsGroup )
                .If( () => ClassProvider.DropdownShow(), () => IsOpen )
                .If( () => ClassProvider.DropdownRight(), () => IsRightAligned )
                .If( () => ClassProvider.DropdownDirection( Direction ), () => Direction != Direction.Down );

            base.RegisterClasses();
        }

        public void Open()
        {
            var temp = IsOpen;

            IsOpen = true;

            if ( temp != IsOpen ) // used to prevent toggle event call if Open() is called multiple times
                Toggled?.Invoke( IsOpen );

            StateHasChanged();
        }

        public void Close()
        {
            var temp = IsOpen;

            IsOpen = false;

            if ( temp != IsOpen ) // used to prevent toggle event call if Close() is called multiple times
                Toggled?.Invoke( IsOpen );

            StateHasChanged();
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            Toggled?.Invoke( IsOpen );

            StateHasChanged();
        }

        /// <summary>
        /// Links the dropdown-menu with this dropdown.
        /// </summary>
        /// <param name="dropdownMenu">Dropdown-menu to link.</param>
        internal void Hook( BaseDropdownMenu dropdownMenu )
        {
            this.dropdownMenu = dropdownMenu;
        }

        internal void Hook( BaseDropdownToggle dropdownToggle )
        {
            this.dropdownToggle = dropdownToggle;
        }

        /// <summary>
        /// Registers a child button reference.
        /// </summary>
        /// <param name="button">Button to register.</param>
        internal void Register( BaseButton button )
        {
            if ( button == null )
                return;

            if ( registeredButtons == null )
                registeredButtons = new List<BaseButton>();

            if ( !registeredButtons.Contains( button ) )
            {
                registeredButtons.Add( button );

                ClassMapper.Dirty();

                if ( registeredButtons?.Count > 1 ) // must find a better way to refresh dropdown
                    StateHasChanged();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes the drop down to behave as a group for buttons(used for the split-button behaviour).
        /// </summary>
        internal bool IsGroup => Buttons != null || registeredButtons?.Count > 1;

        /// <summary>
        /// Handles the visibility of dropdown items.
        /// </summary>
        [Parameter]
        internal bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                if ( dropdownMenu != null )
                    dropdownMenu.IsOpen = value;

                if ( dropdownToggle != null )
                    dropdownToggle.IsOpen = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Right aligned dropdown menu.
        /// </summary>
        [Parameter]
        protected bool IsRightAligned
        {
            get => isRightAligned;
            set
            {
                isRightAligned = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Dropdown-menu slide direction.
        /// </summary>
        [Parameter]
        protected Direction Direction
        {
            get => direction;
            set
            {
                direction = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseButtons Buttons { get; set; }

        [Parameter] protected Action<bool> Toggled { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
