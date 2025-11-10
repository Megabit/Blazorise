#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="MemoInput"/> JS module.
/// </summary>
public class JSMemoInputModule : BaseJSModule, IJSMemoInputModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSMemoInputModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId, MemoInputJSOptions options )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId, options );

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, MemoInputUpdateJSOptions options )
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
    public override string ModuleFileName => $"./_content/Blazorise/memoInput.js?v={VersionProvider.Version}";

    #endregion
}