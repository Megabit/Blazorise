#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the dropdown JS module.
/// </summary>
public class JSDropdownModule : BaseJSModule, IJSDropdownModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSDropdownModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId, string targetElementId, string menuElementId, DropdownJSOptions options )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId, targetElementId, menuElementId, options );

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask Show( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "show", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask Hide( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "hide", elementRef, elementId );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/dropdown.js?v={VersionProvider.Version}";

    #endregion
}