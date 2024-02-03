using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Captcha;

public abstract class BaseCaptcha : BaseComponent
{
    /// <summary>
    /// The current state of the Captcha.
    /// </summary>
    public CaptchaState State { get; private set; } = new();

    /// <summary>
    /// A Captcha success event. May prove a response token for further validation.
    /// </summary>
    [Parameter] public EventCallback<string> OnSuccess { get; set; }

    /// <summary>
    /// The Captcha expired event.
    /// </summary>
    [Parameter] public EventCallback OnExpired { get; set; }


    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Render();
        }
    }

    public abstract Task Render();

    public async Task SetSuccess( string response )
    {
        State.Valid = true;
        State.Response = response;

        if ( OnSuccess.HasDelegate )
        {
            await OnSuccess.InvokeAsync( response );
        }

    }

    public async Task SetExpired()
    {
        State.Valid = false;
        State.Response = string.Empty;

        if ( OnExpired.HasDelegate )
        {
            await OnExpired.InvokeAsync();
        }
    }

    [JSInvokable, EditorBrowsable( EditorBrowsableState.Never )]
    public async Task OnSuccessHandler( string response )
    {
        await SetSuccess( response );
    }

    [JSInvokable, EditorBrowsable( EditorBrowsableState.Never )]
    public async Task OnExpiredHandler()
    {
        await SetExpired();
    }

}
