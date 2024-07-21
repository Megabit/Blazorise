using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Material.Modules;

internal class MaterialJSModalModule : JSModalModule
{
    public MaterialJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Material/modal.js?v={VersionProvider.Version}";
}