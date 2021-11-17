using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules
{
    public class BootstrapJSTooltipModule : JSTooltipModule
    {
        public BootstrapJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Bootstrap/tooltip.js?v={VersionProvider.Version}";
    }
}
