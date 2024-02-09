using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Captcha;

public abstract class Captcha : BaseComponent
{
    /// <summary>
    /// The current state of the Captcha.
    /// </summary>
    public CaptchaState State { get; private set; } = new();

    /// <summary>
    /// A Captcha solved event. 
    /// Provides contextual information about the Captcha state after the user has solved.
    /// </summary>
    [Parameter] public EventCallback<CaptchaState> OnSolved { get; set; }

    /// <summary>
    /// A Captcha validation event. Further validation may be performed here.
    /// </summary>
    [Parameter] public Func<CaptchaState, Task<bool>> OnValidate { get; set; }

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

    public async Task SetSolved( string response )
    {
        State.Response = response;

        if ( OnValidate is null )
        {
            State.Valid = true;
        }
        else
        {
            State.Valid = await OnValidate.Invoke( State );
        }

        if ( OnSolved.HasDelegate )
        {
            await OnSolved.InvokeAsync( State );
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

    public abstract Task Render();
    public abstract Task Reset();

}
