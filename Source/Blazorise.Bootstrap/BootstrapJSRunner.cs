#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bootstrap
{
    public partial class BootstrapJSRunner : JSRunner
    {
        private const string BOOTSTRAP_NAMESPACE = "blazoriseBootstrap";

        public BootstrapJSRunner( IJSRuntime runtime )
            : base( runtime )
        {
        }

        public override ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.tooltip.initialize", elementRef, elementId, options );
        }

        public override ValueTask<bool> OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            return Runtime.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.modal.open", elementRef, scrollToTop );
        }

        public override ValueTask<bool> CloseModal( ElementReference elementRef )
        {
            return Runtime.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.modal.close", elementRef );
        }

        //public override Task<bool> ActivateDatePicker( string elementId )
        //{
        //    return JSRuntime.Current.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.activateDatePicker", elementId );
        //}
    }
}
