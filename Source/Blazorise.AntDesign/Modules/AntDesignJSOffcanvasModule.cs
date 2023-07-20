using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules;

internal class AntDesignJSOffcanvasModule : JSOffcanvasModule
{
    public AntDesignJSOffcanvasModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.AntDesign/offcanvas.js?v={VersionProvider.Version}";
}