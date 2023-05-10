using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Framework7.Modules;

internal class Framework7JSTooltipModule : JSTooltipModule
{
    public Framework7JSTooltipModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Framework7/tooltip.js?v={VersionProvider.Version}";
}