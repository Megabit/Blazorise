using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material5.Modules
{
    internal class MaterialJSTooltipModule : JSTooltipModule
    {
        public MaterialJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Material5/tooltip.js?v={VersionProvider.Version}";
    }
}
