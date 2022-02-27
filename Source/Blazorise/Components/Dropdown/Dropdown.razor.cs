#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Dropdown is toggleable, contextual overlay for displaying lists of links and more.
    /// </summary>
    public partial class Dropdown : BaseComponent, IDisposable
    {
        #region Members

        /// <summary>
        /// State object used to holds the dropdown state.
        /// </summary>
        private DropdownState state = new()
        {
            Direction = Direction.Default,
        };

        /// <summary>
        /// A list of all buttons placed inside of this dropdown.
        /// </summary>
        private List<Button> buttonList;

        /// <summary>
        /// The direct Dropdown child of this dropdown.
        /// </summary>
        private Dropdown childDropdown;

        /// <summary>
        /// A list of all DropdownMenu placed inside of this dropdown.
        /// </summary>
        private List<DropdownMenu> childrenDropdownMenus;

        #endregion

        #region Methods

        /// <summary>
        /// Adds child DropdownMenu to internal collection.
        /// </summary>
        /// <param name="dropdownMenu"></param>
        internal protected void AddDropdownMenu( DropdownMenu dropdownMenu )
        {
            childrenDropdownMenus ??= new();
            childrenDropdownMenus.Add( dropdownMenu );
        }

        /// <summary>
        /// Removes child DropdownMenu from internal collection.
        /// </summary>
        /// <param name="dropdownMenu"></param>
        internal protected bool RemoveDropdownMenu( DropdownMenu dropdownMenu )
            => childrenDropdownMenus.Remove( dropdownMenu );

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( ParentDropdown != null )
            {
                ParentDropdown.NotifyChildDropdownInitialized( this );
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void OnAfterRender( bool firstRender )
        {
            if ( firstRender && buttonList?.Count > 0 )
            {
                DirtyClasses();
                DirtyStyles();

                InvokeAsync( StateHasChanged );
            }

            WasJustToggled = false;

            base.OnAfterRender( firstRender );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Dropdown( IsDropdownSubmenu ) );
            builder.Append( ClassProvider.DropdownGroup(), IsGroup );
            builder.Append( ClassProvider.DropdownShow(), Visible );
            builder.Append( ClassProvider.DropdownRight(), RightAligned );
            builder.Append( ClassProvider.DropdownDirection( GetDropdownDirection() ), Direction != Direction.Down );
            builder.Append( ClassProvider.DropdownTableResponsive(), InResponsiveTable );

            base.BuildClasses( builder );
        }

        private Direction GetDropdownDirection()
            => IsDropdownSubmenu && Direction == Direction.Default ? Direction.End : Direction;

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentDropdown != null )
                {
                    ParentDropdown.NotifyChildDropdownRemoved( this );
                }
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Show the dropdown menu.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Show()
        {
            // used to prevent toggle event call if Open() is called multiple times
            if ( Visible )
                return Task.CompletedTask;

            Visible = true;

            return InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Hide the dropdown menu.
        /// </summary>
        /// <param name="hideAll">Indicates if we need to hide current dropdown menu and all its parent dropdown menus.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Hide( bool hideAll = false )
        {
            // used to prevent toggle event call if Close() is called multiple times
            if ( !Visible )
                return;

            Visible = false;

            if ( ParentDropdown is not null && ( ParentDropdown.ShouldClose || hideAll ) )
                await ParentDropdown.Hide( hideAll );

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Handles the onmouseleave event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected void OnMouseLeaveHandler()
        {
            ShouldClose = true;
        }

        /// <summary>
        /// Handles the onmouseenter event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected void OnMouseEnterHandler()
        {
            ShouldClose = false;
        }

        /// <summary>
        /// Toggle the visibility of the dropdown menu.
        /// </summary>
        /// <param name="dropdownToggleElementId"></param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Toggle( string dropdownToggleElementId )
        {
            SetWasJustToggled( true );
            SetSelectedDropdownElementId( dropdownToggleElementId );
            Visible = !Visible;

            return InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Sets the WasToggled Flag on the current Dropdown and every existing ParentDropdown.
        /// </summary>
        /// <param name="wasToggled"></param>
        internal void SetWasJustToggled( bool wasToggled )
        {
            WasJustToggled = wasToggled;

            ParentDropdown?.SetWasJustToggled( wasToggled );
        }

        /// <summary>
        /// Sets Selected Dropdown Toggle ElementId
        /// </summary>
        /// <param name="dropdownToggleElementId"></param>
        internal void SetSelectedDropdownElementId( string dropdownToggleElementId )
        {
            SelectedDropdownElementId = dropdownToggleElementId;

            if ( ParentDropdown is not null )
                ParentDropdown.SetSelectedDropdownElementId( dropdownToggleElementId );
        }

        /// <summary>
        /// Notifies the <see cref="Dropdown"/> that it has a child button component.
        /// </summary>
        /// <param name="button">Reference to the <see cref="Button"/> that is placed inside of this <see cref="Dropdown"/>.</param>
        internal void NotifyButtonInitialized( Button button )
        {
            if ( button == null )
                return;

            buttonList ??= new();

            if ( !buttonList.Contains( button ) )
            {
                buttonList.Add( button );
            }
        }

        /// <summary>
        /// Notifies the <see cref="Dropdown"/> that it's a child button component should be removed.
        /// </summary>
        /// <param name="button">Reference to the <see cref="Button"/> that is placed inside of this <see cref="Dropdown"/>.</param>
        internal void NotifyButtonRemoved( Button button )
        {
            if ( button == null )
                return;

            if ( buttonList != null && buttonList.Contains( button ) )
            {
                buttonList.Remove( button );
            }
        }

        /// <summary>
        /// Notifies the <see cref="Dropdown"/> that it has a child dropdown component.
        /// </summary>
        /// <param name="dropdown">Reference to the <see cref="Dropdown"/> that is placed inside of this <see cref="Dropdown"/>.</param>
        internal void NotifyChildDropdownInitialized( Dropdown dropdown )
        {
            if ( childDropdown == null )
                childDropdown = dropdown;
        }

        /// <summary>
        /// Notifies the <see cref="Dropdown"/> that it's a child dropdown component should be removed.
        /// </summary>
        /// <param name="dropdown">Reference to the <see cref="Dropdown"/> that is placed inside of this <see cref="Dropdown"/>.</param>
        internal void NotifyChildDropdownRemoved( Dropdown dropdown )
        {
            childDropdown = null;
        }

        /// <summary>
        /// Handles the styles based on the visibility flag.
        /// </summary>
        /// <param name="visible">Dropdown menu visibility flag.</param>
        private void HandleVisibilityStyles( bool visible )
        {
            DirtyClasses();
            DirtyStyles();
        }

        /// <summary>
        /// Handles all the events in this <see cref="Dropdown"/> based on the visibility flag.
        /// </summary>
        /// <param name="visible">Dropdown menu visibility flag.</param>
        private void HandleVisibilityEvents( bool visible )
        {
            VisibleChanged.InvokeAsync( visible );

            if ( childrenDropdownMenus is not null )
            {
                foreach ( var dropdownMenu in childrenDropdownMenus )
                {
                    dropdownMenu.OnVisibleChanged( visible );
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Keeps track whether the Dropdown is in a state where it should close.
        /// </summary>
        internal bool ShouldClose { get; set; } = false;

        /// <summary>
        /// Keeps track whether the Dropdown was just toggled, ignoring possible DropdownItem clicks which would otherwise close the dropdown.
        /// </summary>
        internal bool WasJustToggled { get; set; } = false;

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets the reference to the <see cref="DropdownState"/>.
        /// </summary>
        protected DropdownState State => state;

        /// <summary>
        /// Makes the drop down to behave as a group for buttons(used for the split-button behaviour).
        /// </summary>
        protected internal bool IsGroup => ParentButtons != null || buttonList?.Count >= 1;

        /// <summary>
        /// Returns true if the dropdown is placed inside of another dropdown.
        /// </summary>
        protected internal bool IsDropdownSubmenu => ParentDropdown != null;

        /// <summary>
        /// Returns true if this dropdown contains any child dropdown.
        /// </summary>
        protected internal bool HasSubmenu => childDropdown != null;

        /// <summary>
        /// Returns true if dropdown is placed inside of responsive table.
        /// </summary>
        protected internal bool InResponsiveTable => ParentTable?.Responsive == true;

        /// <summary>
        /// Tracks the last DropdownToggle Element Id that acted.
        /// </summary>
        public string SelectedDropdownElementId { get; set; }

        /// <summary>
        /// If true, a dropdown menu will be visible.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => state.Visible;
            set
            {
                // prevent from calling the same code multiple times
                if ( value == state.Visible )
                    return;

                state = state with { Visible = value };

                HandleVisibilityStyles( value );
                HandleVisibilityEvents( value );
            }
        }

        /// <summary>
        /// If true, a dropdown menu will be right aligned.
        /// </summary>
        [Parameter]
        public bool RightAligned
        {
            get => state.RightAligned;
            set
            {
                state = state with { RightAligned = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// If true, dropdown would not react to button click.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => state.Disabled;
            set
            {
                state = state with { Disabled = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Dropdown-menu slide direction.
        /// </summary>
        [Parameter]
        public Direction Direction
        {
            get => state.Direction;
            set
            {
                state = state with { Direction = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs after the dropdown menu visibility has changed.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Gets or sets the cascaded parent buttons component.
        /// </summary>
        [CascadingParameter] protected Buttons ParentButtons { get; set; }

        /// <summary>
        /// Gets or sets the cascaded parent table component.
        /// </summary>
        [CascadingParameter] protected Table ParentTable { get; set; }

        /// <summary>
        /// Gets or sets the cascaded parent Dropdown component.
        /// </summary>
        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Dropdown"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
