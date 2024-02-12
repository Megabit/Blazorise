#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Captcha;

/// <summary>
/// Component that acts as a wrapper for all Captcha implementations.
/// </summary>
public partial class Captcha : BaseComponent
{
    #region Members

    /// <summary>
    /// The current state of the Captcha.
    /// </summary>
    public CaptchaState State { get; private set; } = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await Initialize();
        }
    }

    /// <summary>
    /// Initialize the Captcha.
    /// </summary>
    /// <returns></returns>
    protected virtual Task Initialize()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the Captcha as Solved, and invokes the OnSolved event.
    /// </summary>
    /// <param name="response">The contextual captcha response.</param>
    /// <returns></returns>
    protected async Task SetSolved( string response )
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
    protected async Task SetExpired()
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
    /// Resets the Captcha.
    /// </summary>
    /// <returns></returns>
    public virtual Task Reset()
    {
        State.Valid = false;
        State.Response = string.Empty;

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// A Captcha solved event. 
    /// <para>Provides contextual information about the Captcha state after the user has solved.</para>
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

    #endregion
}
