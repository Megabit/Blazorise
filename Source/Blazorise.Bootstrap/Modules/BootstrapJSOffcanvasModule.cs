using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bootstrap.Modules;

internal class BootstrapJSOffcanvasModule : JSOffcanvasModule
{
    public BootstrapJSOffcanvasModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bootstrap5/offcanvas.js?v={VersionProvider.Version}";
}