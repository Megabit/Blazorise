#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Animate;

/// <summary>
/// Default implementation of the Animate JS module.
/// </summary>
public class JSAnimateModule : BaseJSModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSAnimateModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Runs the animation for the rendered element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="options">Animation options.</param>
    /// <returns>True if the animation completed or was started successfully.</returns>
    public virtual ValueTask<bool> Animate( ElementReference elementRef, object options )
        => InvokeAsync<bool>( "animate", elementRef, options );

    /// <summary>
    /// Disposes any browser-side animation state for the rendered element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask DisposeElement( ElementReference elementRef )
        => InvokeSafeVoidAsync( "dispose", elementRef );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Animate/animate.js?v={VersionProvider.Version}";

    #endregion
}