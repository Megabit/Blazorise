#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Frolic
{
    public partial class FrolicJSRunner : Blazorise.JSRunner
    {
        public FrolicJSRunner( IJSRuntime runtime )
            : base( runtime )
        {
        }

        public override ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseFrolic.tooltip.initialize", elementRef, elementId, options );
        }

        public override ValueTask<bool> OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseFrolic.modal.open", elementRef, scrollToTop );
        }

        public override ValueTask<bool> CloseModal( ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseFrolic.modal.close", elementRef );
        }
    }
}
