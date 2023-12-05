#region Using directives
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Tests.bUnit;

public class MockJsModalModule : JSModalModule
{
    public MockJsModalModule( IJSRuntime jsRuntime, IVersionProvider versionProvider ) : base( jsRuntime, versionProvider )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.Bootstrap/modal.js?v={VersionProvider.Version}";
}
