#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bootstrap5
{
    public partial class BootstrapJSRunner : JSRunner
    {
        private const string BOOTSTRAP_NAMESPACE = "blazoriseBootstrap";

        public BootstrapJSRunner( IJSRuntime runtime )
            : base( runtime )
        {
        }

        public override ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"{BOOTSTRAP_NAMESPACE}.tooltip.initialize", elementRef, elementId, options );
        }

        public override ValueTask OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            return Runtime.InvokeVoidAsync( $"{BOOTSTRAP_NAMESPACE}.modal.open", elementRef, scrollToTop );
        }

        public override ValueTask CloseModal( ElementReference elementRef )
        {
            return Runtime.InvokeVoidAsync( $"{BOOTSTRAP_NAMESPACE}.modal.close", elementRef );
        }

        //public override Task<bool> ActivateDatePicker( string elementId )
        //{
        //    return JSRuntime.Current.InvokeAsync<bool>( $"{BOOTSTRAP_NAMESPACE}.activateDatePicker", elementId );
        //}
    }
}
