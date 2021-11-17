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

        public override ValueTask InitializeTooltip( ElementReference elementRef, string elementId, object options )
        {
            return Runtime.InvokeVoidAsync( $"blazoriseFrolic.tooltip.initialize", elementRef, elementId, options );
        }

        public override ValueTask OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            return Runtime.InvokeVoidAsync( $"blazoriseFrolic.modal.open", elementRef, scrollToTop );
        }

        public override ValueTask CloseModal( ElementReference elementRef )
        {
            return Runtime.InvokeVoidAsync( $"blazoriseFrolic.modal.close", elementRef );
        }
    }
}
