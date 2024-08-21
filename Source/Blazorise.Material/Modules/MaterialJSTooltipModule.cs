using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material.Modules;

internal class MaterialJSTooltipModule : JSTooltipModule
{
    public MaterialJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Material/tooltip.js?v={VersionProvider.Version}";
}