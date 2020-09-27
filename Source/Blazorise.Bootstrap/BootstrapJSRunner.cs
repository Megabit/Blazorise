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

        public override ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.tooltip.initialize", elementRef, elementId );
        }

        public override ValueTask<bool> OpenModal( ElementReference elementRef, string elementId, bool scrollToTop )
        {
            return runtime.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.modal.open", elementRef, elementId, scrollToTop );
        }

        public override ValueTask<bool> CloseModal( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.modal.close", elementRef, elementId );
        }

        //public override Task<bool> ActivateDatePicker( string elementId )
        //{
        //    return JSRuntime.Current.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.activateDatePicker", elementId );
        //}
    }
}
