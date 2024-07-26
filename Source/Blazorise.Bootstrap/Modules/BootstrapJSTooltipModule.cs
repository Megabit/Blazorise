using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules;

public class BootstrapJSTooltipModule : JSTooltipModule
{
    public BootstrapJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bootstrap/tooltip.js?v={VersionProvider.Version}";
}