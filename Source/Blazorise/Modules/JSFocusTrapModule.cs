#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the focus trap JS module.
/// </summary>
public class JSFocusTrapModule : BaseJSModule, IJSFocusTrapModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSFocusTrapModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask Focus( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "focus", elementRef, elementId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/focusTrap.js?v={VersionProvider.Version}";

    #endregion
}