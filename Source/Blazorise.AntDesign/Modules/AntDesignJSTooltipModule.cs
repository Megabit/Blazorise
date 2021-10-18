using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules
{
    internal class AntDesignJSTooltipModule : JSTooltipModule
    {
        public AntDesignJSTooltipModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.AntDesign/tooltip.js";
    }
}
