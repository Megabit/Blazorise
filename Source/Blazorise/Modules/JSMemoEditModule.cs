#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="MemoEdit"/> JS module.
/// </summary>
public class JSMemoEditModule : BaseJSModule, IJSMemoEditModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSMemoEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId, MemoEditInitializeJSOptions options )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, MemoEditUpdateJSOptions options )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );
    }

    /// <inheritdoc/>
    public virtual ValueTask RecalculateAutoHeight( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "recalculateAutoHeight", elementRef, elementId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/memoEdit.js?v={VersionProvider.Version}";

    #endregion
}