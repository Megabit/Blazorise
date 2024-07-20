#region Using directives
using System.Threading.Tasks;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Cropper;

internal class JSReCaptchaModule : BaseJSModule, IJSDestroyableModule
{
    public JSReCaptchaModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    public ValueTask Initialize( DotNetObjectReference<ReCaptcha> dotNetObjectReference, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );

    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    public ValueTask Submit( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "execute", elementRef, elementId );

    public ValueTask Reset( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "reset", elementRef, elementId );

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Captcha.ReCaptcha/blazorise.recaptcha.js?v={VersionProvider.Version}";
}
