using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Framework7.Modules;

internal class Framework7JSModalModule : JSModalModule
{
    public Framework7JSModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Framework7/modal.js?v={VersionProvider.Version}";
}