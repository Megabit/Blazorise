﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class DropdownToggle : BaseComponent, ICloseActivator
    {
        #region Members

        private bool split;

        private bool jsRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        private DropdownStore parentDropdownStore;

        #endregion

        #region Methods

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownToggle() );
            builder.Append( ClassProvider.DropdownToggleColor( Color ), Color != Color.None && !Outline );
            builder.Append( ClassProvider.DropdownToggleOutline( Color ), Color != Color.None && Outline );
            builder.Append( ClassProvider.DropdownToggleSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.DropdownToggleSplit(), Split );
            builder.Append( ClassProvider.DropdownToggleIcon( IsToggleIconVisible ) );

            base.BuildClasses( builder );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( jsRegistered )
                {
                    jsRegistered = false;

                    if ( Rendered )
                    {
                        _ = JSRunner.UnregisterClosableComponent( this );
                    }
                }
                if ( Rendered )
                {
                    JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
                }
            }

            base.Dispose( disposing );
        }

        protected Task ClickHandler()
        {
            ParentDropdown?.Toggle();

            return Task.CompletedTask;
        }

        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason )
        {
            return Task.FromResult( closeReason == CloseReason.EscapeClosing || elementId != ElementId );
        }

        public Task Close( CloseReason closeReason )
        {
            ParentDropdown?.Hide();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            _ = JSRunner.Focus( ElementRef, ElementId, scrollToElement );
        }

        #endregion

        #region Properties

        protected bool IsGroup => ParentDropdown?.IsGroup == true;

        /// <summary>
        /// Should the toggle icon be drawn
        /// </summary>
        protected bool IsToggleIconVisible => ToggleIconVisible.GetValueOrDefault( Theme?.DropdownOptions?.ToggleIconVisible ?? true );

        /// <summary>
        /// Gets or sets the dropdown color.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.None;

        /// <summary>
        /// Gets or sets the dropdown size.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.None;

        /// <summary>
        /// Button outline.
        /// </summary>
        [Parameter] public bool Outline { get; set; }

        /// <summary>
        /// Indicates that a toggle should act as a split button.
        /// </summary>
        [Parameter]
        public bool Split
        {
            get => split;
            set
            {
                split = value;

                DirtyClasses();
            }
        }

        [CascadingParameter]
        protected DropdownStore ParentDropdownStore
        {
            get => parentDropdownStore;
            set
            {
                if ( parentDropdownStore == value )
                    return;

                parentDropdownStore = value;

                if ( parentDropdownStore.Visible )
                {
                    jsRegistered = true;

                    if ( Rendered )
                    {
                        JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementId );
                    }
                }
                else
                {
                    jsRegistered = false;

                    if ( Rendered )
                    {
                        JSRunner.UnregisterClosableComponent( this );
                    }
                }

                DirtyClasses();
            }
        }

        [CascadingParameter] protected Dropdown ParentDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dropdown toggle icon is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if [show toggle]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Default: True</remarks>
        [Parameter] public bool? ToggleIconVisible { get; set; }

        /// <summary>
        /// The applied theme.
        /// </summary>
        [CascadingParameter] protected Theme Theme { get; set; }

        #endregion
    }
}
