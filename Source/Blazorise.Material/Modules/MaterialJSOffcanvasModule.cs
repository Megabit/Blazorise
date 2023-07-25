using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material.Modules;

internal class MaterialJSOffcanvasModule : JSOffcanvasModule
{
    public MaterialJSOffcanvasModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Material/offcanvas.js?v={VersionProvider.Version}";
}