using Blazorise.Modules;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules
{
    internal class AntDesignJSFileEditModule : JSFileEditModule
    {
        public AntDesignJSFileEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        public override string ModuleFileName => $"./_content/Blazorise.AntDesign/fileEdit.js?v={VersionProvider.Version}";
    }
}
