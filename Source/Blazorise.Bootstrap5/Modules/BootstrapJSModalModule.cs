using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap5.Modules
{
    internal class BootstrapJSModalModule : JSModalModule
    {
        public BootstrapJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Bootstrap5/modal.js?v={VersionProvider.Version}";
    }
}
