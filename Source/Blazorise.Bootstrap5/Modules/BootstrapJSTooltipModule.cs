using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap5.Modules;

internal class BootstrapJSTooltipModule : JSTooltipModule
{
    public BootstrapJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bootstrap5/tooltip.js?v={VersionProvider.Version}";
}