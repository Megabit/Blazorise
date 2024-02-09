using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Captcha.ReCaptcha;
/// <summary>
/// An implementation of Google's reCAPTCHA:
/// <para>https://www.google.com/recaptcha/about/</para>
/// </summary>
public partial class ReCaptcha : Captcha
{
    [Inject] public IJSRuntime JsRuntime { get; set; }
    [Inject] public ReCaptchaOptions Options { get; set; }

    private DotNetObjectReference<ReCaptcha> _dotNetObjectReference;

    protected override Task OnInitializedAsync()
    {
        _dotNetObjectReference = DotNetObjectReference.Create( this );
        ElementId = ElementId ?? IdGenerator.Generate;
        return base.OnInitializedAsync();
    }

    public override async Task Render()
    {
        await JsRuntime.InvokeVoidAsync( "renderReCAPTCHA", _dotNetObjectReference, ElementId, Options.SiteKey, Options.Theme.ToString( "g" ), Options.Size.ToString( "g" ), Options.LanguageCode );
    }

    public override async Task Reset()
    {
        await JsRuntime.InvokeVoidAsync( "resetReCAPTCHA" );
    }

    [JSInvokable, EditorBrowsable( EditorBrowsableState.Never )]
    public async Task OnSuccessHandler( string response )
    {
        await SetSolved( response );
    }

    [JSInvokable, EditorBrowsable( EditorBrowsableState.Never )]
    public async Task OnExpiredHandler()
    {
        await SetExpired();
    }
}
