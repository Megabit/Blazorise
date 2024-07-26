#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="InputMask"/> JS module.
/// </summary>
public class JSInputMaskModule : BaseJSModule, IJSInputMaskModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSInputMaskModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( DotNetObjectReference<InputMask> dotNetObjectRef, ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask ExtendAliases( ElementReference elementRef, string elementId, object aliasOptions )
        => InvokeSafeVoidAsync( "extendAliases", elementRef, elementId, aliasOptions );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/inputMask.js?v={VersionProvider.Version}";

    #endregion
}