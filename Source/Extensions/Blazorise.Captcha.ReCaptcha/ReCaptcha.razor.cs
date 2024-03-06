#region Using directives
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Blazorise.Cropper;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Captcha.ReCaptcha;

/// <summary>
/// An implementation of Google's reCAPTCHA:
/// <para>https://www.google.com/recaptcha/about/</para>
/// </summary>
public partial class ReCaptcha : Captcha, IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<ReCaptcha> dotNetObjectReference;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        dotNetObjectReference = DotNetObjectReference.Create( this );
        ElementId ??= IdGenerator.Generate;
        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );
            await JSModule.SafeDisposeAsync();
            dotNetObjectReference?.Dispose();
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override async Task Initialize()
    {
        JSModule ??= new JSReCaptchaModule( JSRuntime, VersionProvider );

        await JSModule.Initialize( dotNetObjectReference, ElementRef, ElementId,
            new
            {
                SiteKey = Options.SiteKey,
                Theme = Options.Theme.ToString( "g" ).ToLowerInvariant(),
                Size = Options.Size.ToString( "g" ).ToLowerInvariant(),
                Badge = Options.BadgePosition.ToString( "g" ).ToLowerInvariant(),
                Language = Options.LanguageCode
            } );
    }

    /// <inheritdoc/>
    public override async Task Submit()
    {
        await JSModule.Submit( ElementRef, ElementId );
    }

    /// <inheritdoc/>
    public override async Task Reset()
    {
        State.Valid = false;
        State.Response = string.Empty;
        await JSModule.Reset( ElementRef, ElementId );
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

    #endregion

    #region Properties

    internal JSReCaptchaModule JSModule { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] public ReCaptchaOptions Options { get; set; }

    #endregion
}
