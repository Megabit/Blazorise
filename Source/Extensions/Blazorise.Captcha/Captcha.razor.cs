using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Captcha;

public partial class Captcha : BaseComponent
{
    /// <summary>
    /// The current state of the Captcha.
    /// </summary>
    public CaptchaState State { get; private set; } = new();

    /// <summary>
    /// A Captcha solved event. 
    /// Provides contextual information about the Captcha state after the user has solved.
    /// </summary>
    [Parameter] public EventCallback<CaptchaState> Solved { get; set; }

    /// <summary>
    /// A Captcha validation event. Further validation may be performed here.
    /// </summary>
    [Parameter] public Func<CaptchaState, Task<bool>> Validate { get; set; }

    /// <summary>
    /// The Captcha expired event.
    /// </summary>
    [Parameter] public EventCallback Expired { get; set; }


    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Initialize();
        }
    }

    /// <summary>
    /// Sets the Captcha as Solved, and invokes the OnSolved event.
    /// </summary>
    /// <param name="response">The contextual captcha response.</param>
    /// <returns></returns>
    public async Task SetSolved( string response )
    {
        State.Response = response;

        if ( Validate is null )
        {
            State.Valid = true;
        }
        else
        {
            State.Valid = await Validate.Invoke( State );
        }

        if ( Solved.HasDelegate )
        {
            await Solved.InvokeAsync( State );
        }
    }

    /// <summary>
    /// Sets the Captcha as Expired, and invokes the OnExpired event.
    /// </summary>
    /// <returns></returns>
    public async Task SetExpired()
    {
        State.Valid = false;
        State.Response = string.Empty;

        if ( Expired.HasDelegate )
        {
            await Expired.InvokeAsync();
        }
    }

    /// <summary>
    /// Submits the Captcha.
    /// </summary>
    /// <returns></returns>
    public virtual Task Submit()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initialize the Captcha.
    /// </summary>
    /// <returns></returns>
    public virtual Task Initialize()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Resets the Captcha.
    /// </summary>
    /// <returns></returns>
    public virtual Task Reset()
    {
        State.Valid = false;
        State.Response = string.Empty;
        return Task.CompletedTask;
    }

}
