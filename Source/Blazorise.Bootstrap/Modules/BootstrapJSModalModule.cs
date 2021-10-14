using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules
{
    internal class BootstrapJSModalModule : JSModalModule
    {
        public BootstrapJSModalModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bootstrap/modal.js";
    }
}
