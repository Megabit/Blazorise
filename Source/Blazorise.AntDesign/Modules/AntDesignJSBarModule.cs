using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.AntDesign.Modules;

public class AntDesignJSBarModule : BaseJSModule
{
    public AntDesignJSBarModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public ValueTask UpdatePopupPlacement( ElementReference elementRef, bool visible )
        => InvokeSafeVoidAsync( "updatePopupPlacement", elementRef, visible );

    public ValueTask ResetPopupPlacement( ElementReference elementRef )
        => InvokeSafeVoidAsync( "resetPopupPlacement", elementRef );

    public override string ModuleFileName => $"./_content/Blazorise.AntDesign/bar.js?v={VersionProvider.Version}";
}