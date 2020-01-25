#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Dropdown : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isRightAligned;

        private Direction direction = Blazorise.Direction.Down;

        public event EventHandler<DropdownStateEventArgs> StateChanged;

        private List<Button> registeredButtons;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Dropdown() );
            builder.Append( ClassProvider.DropdownGroup(), IsGroup );
            builder.Append( ClassProvider.DropdownShow(), IsOpen );
            builder.Append( ClassProvider.DropdownRight(), IsRightAligned );
            builder.Append( ClassProvider.DropdownDirection( Direction ), Direction != Direction.Down );

            base.BuildClasses( builder );
        }

        public void Open()
        {
            // used to prevent toggle event call if Open() is called multiple times
            if ( IsOpen )
                return;

            IsOpen = true;
            Toggled.InvokeAsync( IsOpen );

            StateHasChanged();
        }

        public void Close()
        {
            // used to prevent toggle event call if Close() is called multiple times
            if ( !IsOpen )
                return;

            IsOpen = false;
            Toggled.InvokeAsync( IsOpen );

            StateHasChanged();
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            Toggled.InvokeAsync( IsOpen );

            StateHasChanged();
        }

        /// <summary>
        /// Registers a child button reference.
        /// </summary>
        /// <param name="button">Button to register.</param>
        internal void Register( Button button )
        {
            if ( button == null )
                return;

            if ( registeredButtons == null )
                registeredButtons = new List<Button>();

            if ( !registeredButtons.Contains( button ) )
            {
                registeredButtons.Add( button );

                DirtyClasses();

                if ( registeredButtons?.Count >= 1 ) // must find a better way to refresh dropdown
                    StateHasChanged();
            }
        }

        internal void UnRegister( Button button )
        {
            if ( button == null )
                return;

            if ( registeredButtons != null && registeredButtons.Contains( button ) )
            {
                registeredButtons.Remove( button );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes the drop down to behave as a group for buttons(used for the split-button behaviour).
        /// </summary>
        internal bool IsGroup => Buttons != null || registeredButtons?.Count >= 1;

        /// <summary>
        /// Handles the visibility of dropdown menu.
        /// </summary>
        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                // prevent dropdown from calling the same code multiple times
                if ( value == isOpen )
                    return;

                isOpen = value;

                StateChanged?.Invoke( this, new DropdownStateEventArgs( isOpen ) );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Right aligned dropdown menu.
        /// </summary>
        [Parameter]
        public bool IsRightAligned
        {
            get => isRightAligned;
            set
            {
                isRightAligned = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Dropdown-menu slide direction.
        /// </summary>
        [Parameter]
        public Direction Direction
        {
            get => direction;
            set
            {
                direction = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the dropdown menu visibility has changed.
        /// </summary>
        [Parameter] public EventCallback<bool> Toggled { get; set; }

        [CascadingParameter] protected Buttons Buttons { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
