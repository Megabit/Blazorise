using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Tailwind.Modules;

public class TailwindJSModalModule : JSModalModule
{
    public TailwindJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Tailwind/modal.js?v={VersionProvider.Version}";
}