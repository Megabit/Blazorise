using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules
{
    internal class AntDesignJSModalModule : JSModalModule
    {
        public AntDesignJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.AntDesign/modal.js?v={VersionProvider.Version}";
    }
}
