using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules;

internal class BulmaJSModalModule : JSModalModule
{
    public BulmaJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bulma/modal.js?v={VersionProvider.Version}";
}