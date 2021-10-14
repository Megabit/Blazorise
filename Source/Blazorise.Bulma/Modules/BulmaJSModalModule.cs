using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules
{
    internal class BulmaJSModalModule : JSModalModule
    {
        public BulmaJSModalModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.Bulma/modal.js";
    }
}
