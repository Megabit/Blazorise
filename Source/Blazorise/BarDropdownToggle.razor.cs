#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class BarDropdownToggle : BaseComponent, ICloseActivator
    {
        #region Members

        private bool isOpen;

        private bool isRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            // link to the parent component
            BarDropdown?.Hook( this );

            base.OnInitialized();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownToggle() );

            base.BuildClasses( builder );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( isRegistered )
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                    JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
                }
            }

            base.Dispose( disposing );
        }

        protected void ClickHandler()
        {
            BarDropdown?.Toggle();
        }

        public bool SafeToClose( string elementId, bool isEscapeKey )
        {
            return isEscapeKey || elementId != ElementId;
        }

        public void Close()
        {
            BarDropdown?.Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Handles the visibility of dropdown toggle.
        /// </summary>
        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                if ( isOpen )
                {
                    isRegistered = true;

                    JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementId );
                }
                else
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }

                DirtyClasses();
            }
        }

        [CascadingParameter] protected BarDropdown BarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
