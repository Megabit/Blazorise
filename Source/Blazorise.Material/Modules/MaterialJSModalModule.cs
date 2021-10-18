using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material.Modules
{
    internal class MaterialJSModalModule : JSModalModule
    {
        public MaterialJSModalModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Material/modal.js";
    }
}
