using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.Bulma.Modules
{
    internal class BulmaJSDropdownModule : JSDropdownModule
    {
        public BulmaJSDropdownModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.Bulma/dropdown.js?v={VersionProvider.Version}";
    }
}
