#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class BarDropdownToggle : BaseComponent, ICloseActivator
    {
        #region Members

        private BarDropdownStore parentStore;

        private bool isRegistered;

        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new CloseActivatorAdapter( this ) );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownToggle( ParentStore.Mode ) );

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
            ParentBarDropdown?.Toggle();

            return Task.CompletedTask;
        }

        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason )
        {
            return Task.FromResult( closeReason == CloseReason.EscapeClosing || elementId != ElementId );
        }

        public Task Close( CloseReason closeReason )
        {
            ParentBarDropdown?.Hide();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        [CascadingParameter]
        public BarDropdownStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                if ( parentStore.Mode == BarMode.Horizontal )
                {
                    if ( parentStore.Visible )
                    {
                        isRegistered = true;

                        JSRunner.RegisterClosableComponent( dotNetObjectRef, ElementId );
                    }
                    else
                    {
                        isRegistered = false;

                        JSRunner.UnregisterClosableComponent( this );
                    }
                }

                DirtyClasses();
            }
        }

        [CascadingParameter] protected BarDropdown ParentBarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
