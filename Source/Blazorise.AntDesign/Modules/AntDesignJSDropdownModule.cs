using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules
{
    internal class AntDesignJSDropdownModule : JSDropdownModule
    {
        public AntDesignJSDropdownModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.AntDesign/dropdown.js?v={VersionProvider.Version}";
    }
}
