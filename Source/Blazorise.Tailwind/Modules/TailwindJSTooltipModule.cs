using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Tailwind.Modules;

public class TailwindJSTooltipModule : JSTooltipModule
{
    public TailwindJSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Tailwind/tooltip.js?v={VersionProvider.Version}";
}