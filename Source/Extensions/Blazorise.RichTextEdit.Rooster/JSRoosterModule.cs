using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.RichTextEdit.Rooster;

internal class JSRoosterModule : BaseJSModule, IJSDestroyableModule
{
    public JSRoosterModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.RichTextEdit.Rooster/blazorise.rooster.js?v={VersionProvider.Version}";

    public ValueTask Initialize( DotNetObjectReference<RoosterAdapter> adapterReference, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", adapterReference, elementRef, elementId, options );

    public ValueTask InvokeRoosterApi( ElementReference elementRef, string elementId, string action, object options = null )
        => InvokeSafeVoidAsync( "roosterApi", elementRef, elementId, action, options );

    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    public ValueTask<string> GetContent( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<string>( "getContent", elementRef, elementId );

    public async Task SetContent( ElementReference elementRef, string elementId, string content )
        => await InvokeSafeVoidAsync( "setContent", elementRef, elementId, content );
}