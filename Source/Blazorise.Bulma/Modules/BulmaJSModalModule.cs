using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules
{
    internal class BulmaJSModalModule : JSModalModule
    {
        public BulmaJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Bulma/modal.js?v={VersionProvider.Version}";
    }
}
