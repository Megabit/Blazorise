using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules
{
    public class BootstrapJSTooltipModule : JSTooltipModule
    {
        public BootstrapJSTooltipModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bootstrap/tooltip.js";
    }
}
