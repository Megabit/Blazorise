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

        public override Task<bool> InitializeTooltip( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseBulma.tooltip.initialize", elementId, elementRef );
        }

        //public override Task<bool> ActivateDatePicker( string elementId, string formatSubmit )
        //{
        //    Console.WriteLine( "Bulma date not implemented." );
        //    return Task.FromResult( true );
        //}
    }
}
