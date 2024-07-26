#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="Table"/> JS module.
/// </summary>
public class JSTableModule : BaseJSModule, IJSTableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSTableModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask InitializeFixedHeader( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "initializeTableFixedHeader", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask DestroyFixedHeader( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroyTableFixedHeader", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask ScrollTableToPixels( ElementReference elementRef, string elementId, int pixels )
        => InvokeSafeVoidAsync( "fixedHeaderScrollTableToPixels", elementRef, elementId, pixels );

    /// <inheritdoc/>
    public virtual ValueTask ScrollTableToRow( ElementReference elementRef, string elementId, int row )
        => InvokeSafeVoidAsync( "fixedHeaderScrollTableToRow", elementRef, elementId, row );

    /// <inheritdoc/>
    public virtual ValueTask InitializeResizable( ElementReference elementRef, string elementId, TableResizeMode resizeMode )
        => InvokeSafeVoidAsync( "initializeResizable", elementRef, elementId, resizeMode );

    /// <inheritdoc/>
    public virtual ValueTask DestroyResizable( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroyResizable", elementRef, elementId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/table.js?v={VersionProvider.Version}";

    #endregion
}