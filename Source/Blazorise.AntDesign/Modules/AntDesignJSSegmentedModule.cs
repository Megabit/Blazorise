using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules;

public class AntDesignJSSegmentedModule : BaseJSModule
{
    public AntDesignJSSegmentedModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public override string ModuleFileName => $"./_content/Blazorise.AntDesign/segmented.js?v={VersionProvider.Version}";

    public ValueTask Update( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "update", elementRef, elementId );
}