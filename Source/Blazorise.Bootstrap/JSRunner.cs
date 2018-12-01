using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazorise.Bootstrap
{
    public partial class JSRunner : Blazorise.JSRunner
    {
        public override Task<bool> ActivateDatePicker( string elementId )
        {
            return JSRuntime.Current.InvokeAsync<bool>( $"blazoriseBootstrap.activateDatePicker", elementId );
        }
    }
}
