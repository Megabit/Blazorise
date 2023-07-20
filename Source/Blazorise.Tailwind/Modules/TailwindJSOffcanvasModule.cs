using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Tailwind.Modules;

internal class TailwindJSOffcanvasModule : JSOffcanvasModule
{
    public TailwindJSOffcanvasModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Tailwind/offcanvas.js?v={VersionProvider.Version}";
}
