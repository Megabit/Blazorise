using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules
{
    internal class AntDesignJSTooltipModule : JSTooltipModule
    {
        public AntDesignJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.AntDesign/tooltip.js?v={VersionProvider.Version}";
    }
}
