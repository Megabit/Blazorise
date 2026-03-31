using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules;

public class AntDesignJSWaveModule : BaseJSModule
{
    public AntDesignJSWaveModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public ValueTask Initialize( ElementReference elementRef, string targetSelector = null )
        => InvokeSafeVoidAsync( "initialize", elementRef, targetSelector );

    public ValueTask Destroy( ElementReference elementRef )
        => InvokeSafeVoidAsync( "destroy", elementRef );

    public override string ModuleFileName => $"./_content/Blazorise.AntDesign/wave.js?v={VersionProvider.Version}";
}