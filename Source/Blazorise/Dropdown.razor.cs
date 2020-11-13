#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Dropdown : BaseComponent
    {
        #region Members

        private DropdownStore store = new DropdownStore
        {
            Direction = Direction.Down,
        };

        private List<Button> registeredButtons;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Dropdown() );
            builder.Append( ClassProvider.DropdownGroup(), IsGroup );
            builder.Append( ClassProvider.DropdownShow(), Visible );
            builder.Append( ClassProvider.DropdownRight(), RightAligned );
            builder.Append( ClassProvider.DropdownDirection( Direction ), Direction != Direction.Down );

            base.BuildClasses( builder );
        }

        protected override void OnAfterRender( bool firstRender )
        {
            if ( firstRender && registeredButtons?.Count > 0 )
            {
                DirtyClasses();
                StateHasChanged();
            }

            base.OnAfterRender( firstRender );
        }

        public void Show()
        {
            // used to prevent toggle event call if Open() is called multiple times
            if ( Visible )
                return;

            Visible = true;
            Toggled.InvokeAsync( Visible );

            StateHasChanged();
        }

        public void Hide()
        {
            // used to prevent toggle event call if Close() is called multiple times
            if ( !Visible )
                return;

            Visible = false;
            Toggled.InvokeAsync( Visible );

            StateHasChanged();
        }

        public void Toggle()
        {
            Visible = !Visible;
            Toggled.InvokeAsync( Visible );

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

        protected DropdownStore Store => store;

        /// <summary>
        /// Makes the drop down to behave as a group for buttons(used for the split-button behaviour).
        /// </summary>
        internal bool IsGroup => Buttons != null || registeredButtons?.Count >= 1;

        /// <summary>
        /// Handles the visibility of dropdown menu.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => store.Visible;
            set
            {
                store.Visible = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Right aligned dropdown menu.
        /// </summary>
        [Parameter]
        public bool RightAligned
        {
            get => store.RightAligned;
            set
            {
                store.RightAligned = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Right aligned dropdown menu.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => store.Disabled;
            set
            {
                store.Disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Dropdown-menu slide direction.
        /// </summary>
        [Parameter]
        public Direction Direction
        {
            get => store.Direction;
            set
            {
                store.Direction = value;

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
