#region Using directives

using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

#endregion

namespace Blazorise.LottieAnimation;

/// <summary>
/// Default implementation of the lottie JS module.
/// </summary>
public class JSLottieAnimationModule : BaseJSModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSLottieAnimationModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options ) : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes a new Lottie Animation instance
    /// </summary>
    /// <param name="dotNetObjectReference">Reference to the <see cref="LottieAnimation"/> component</param>
    /// <param name="elementRef">Reference to the container element</param>
    /// <param name="elementId">Id of the container element</param>
    /// <param name="options">Animation configuration options</param>
    /// <returns>A <see cref="IJSObjectReference"/> to the Animation object</returns>
    public virtual ValueTask<IJSObjectReference> InitializeAnimation( DotNetObjectReference<LottieAnimation> dotNetObjectReference, ElementReference elementRef, string elementId, object options )
        => InvokeSafeAsync<IJSObjectReference>( "initializeAnimation", dotNetObjectReference, elementRef, elementId, options );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.LottieAnimation/lottie-animation.js?v={VersionProvider.Version}";

    #endregion
}