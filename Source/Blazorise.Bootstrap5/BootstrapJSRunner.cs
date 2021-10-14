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
    }
}
