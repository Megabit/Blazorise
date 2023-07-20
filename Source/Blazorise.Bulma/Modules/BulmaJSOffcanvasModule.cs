using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules;

internal class BulmaJSOffcanvasModule : JSOffcanvasModule
{
    public BulmaJSOffcanvasModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bulma/offcanvas.js?v={VersionProvider.Version}";
}