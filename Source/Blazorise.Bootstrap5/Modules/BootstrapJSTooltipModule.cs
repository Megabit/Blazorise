using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap5.Modules
{
    internal class BootstrapJSTooltipModule : JSTooltipModule
    {
        public BootstrapJSTooltipModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bootstrap5/tooltip.js";
    }
}
