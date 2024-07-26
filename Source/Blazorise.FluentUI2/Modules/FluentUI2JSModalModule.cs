using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.FluentUI2.Modules;

internal class FluentUI2JSModalModule : JSModalModule
{
    public FluentUI2JSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.FluentUI2/modal.js?v={VersionProvider.Version}";
}