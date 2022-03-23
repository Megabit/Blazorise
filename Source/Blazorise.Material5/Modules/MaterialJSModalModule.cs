using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material5.Modules
{
    internal class MaterialJSModalModule : JSModalModule
    {
        public MaterialJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Material5/modal.js?v={VersionProvider.Version}";
    }
}
