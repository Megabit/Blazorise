using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules;

internal class AntDesignJSTooltipModule : JSTooltipModule
{
    public AntDesignJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.AntDesign/tooltip.js?v={VersionProvider.Version}";
}