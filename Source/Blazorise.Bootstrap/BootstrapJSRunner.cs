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
        public BootstrapJSRunner( IJSRuntime runtime )
            : base( runtime )
        {
        }

        public override Task<bool> InitializeTooltip( string elementId, ElementReference elementRef )
        {
            return runtime.InvokeAsync<bool>( $"blazoriseBootstrap.tooltip.initialize", elementId, elementRef );
        }

        //public override Task<bool> ActivateDatePicker( string elementId )
        //{
        //    return JSRuntime.Current.InvokeAsync<bool>( $"blazoriseBootstrap.activateDatePicker", elementId );
        //}
    }
}
