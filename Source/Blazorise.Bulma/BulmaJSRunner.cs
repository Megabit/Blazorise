#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Bulma
{
    public partial class BulmaJSRunner : Blazorise.JSRunner
    {
        public BulmaJSRunner( IJSRuntime runtime )
            : base( runtime )
        {

        }

        public override ValueTask<bool> InitializeTooltip( ElementReference elementRef, string elementId )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseBulma.tooltip.initialize", elementRef, elementId );
        }

        public override ValueTask<bool> OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseBulma.modal.open", elementRef, scrollToTop );
        }

        public override ValueTask<bool> CloseModal( ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseBulma.modal.close", elementRef );
        }

        //public override Task<bool> ActivateDatePicker( string elementId, string formatSubmit )
        //{
        //    Console.WriteLine( "Bulma date not implemented." );
        //    return Task.FromResult( true );
        //}
    }
}
