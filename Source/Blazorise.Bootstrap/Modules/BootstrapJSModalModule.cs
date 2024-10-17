using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules;

public class BootstrapJSModalModule : JSModalModule
{
    public BootstrapJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bootstrap/modal.js?v={VersionProvider.Version}";
}