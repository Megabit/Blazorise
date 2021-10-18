using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules
{
    internal class BulmaJSTooltipModule : JSTooltipModule
    {
        public BulmaJSTooltipModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bulma/tooltip.js";
    }
}
