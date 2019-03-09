#region Using directives
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

        //public override Task<bool> ActivateDatePicker( string elementId )
        //{
        //    return JSRuntime.Current.InvokeAsync<bool>( $"blazoriseBootstrap.activateDatePicker", elementId );
        //}
    }
}
