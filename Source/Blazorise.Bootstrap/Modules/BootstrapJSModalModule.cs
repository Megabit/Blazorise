using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules
{
    public class BootstrapJSModalModule : JSModalModule
    {
        public BootstrapJSModalModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bootstrap/modal.js";
    }
}
