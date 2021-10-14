using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap5.Modules
{
    internal class BootstrapJSModalModule : JSModalModule
    {
        public BootstrapJSModalModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bootstrap5/modal.js";
    }
}
