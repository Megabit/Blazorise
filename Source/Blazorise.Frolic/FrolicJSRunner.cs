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

        public override ValueTask<bool> InitializeTooltip( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseFrolic.tooltip.initialize", elementId, elementRef );
        }
    }
}
