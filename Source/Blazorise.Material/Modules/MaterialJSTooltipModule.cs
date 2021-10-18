using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material.Modules
{
    internal class MaterialJSTooltipModule : JSTooltipModule
    {
        public MaterialJSTooltipModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Material/tooltip.js";
    }
}
