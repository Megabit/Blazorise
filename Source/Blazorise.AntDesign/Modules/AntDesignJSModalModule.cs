using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules;

internal class AntDesignJSModalModule : JSModalModule
{
    public AntDesignJSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.AntDesign/modal.js?v={VersionProvider.Version}";
}