using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules
{
    internal class AntDesignJSModalModule : JSModalModule
    {
        public AntDesignJSModalModule( IJSRuntime jsRuntime ) : base( jsRuntime )
        {
        }

        public override string ModuleFileName => "./_content/Blazorise.AntDesign/modal.js";
    }
}
