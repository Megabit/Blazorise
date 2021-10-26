using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap5.Modules
{
    internal class BootstrapJSTooltipModule : JSTooltipModule
    {
        public BootstrapJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Bootstrap5/tooltip.js?v={VersionProvider.Version}";
    }
}
