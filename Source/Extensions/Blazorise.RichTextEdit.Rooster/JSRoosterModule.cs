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

    public ValueTask Initialize( DotNetObjectReference<RichTextEdit> adapterReference, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", adapterReference, elementRef, elementId, options );

    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => ValueTask.CompletedTask;
}