#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides JavaScript interop functionality for Reporting designer interactions.
/// </summary>
public class JSReportingModule : BaseJSModule
{
    #region Constructors

    /// <summary>
    /// Initializes a new Reporting JavaScript module instance.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime used to execute interop calls.</param>
    /// <param name="versionProvider">Provider used to append the Blazorise version to the module URL.</param>
    /// <param name="options">Blazorise options used by the base module.</param>
    public JSReportingModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Starts document-level band resizing for the active report designer.
    /// </summary>
    /// <typeparam name="TItem">Report data item type.</typeparam>
    /// <param name="dotNetObjectReference">Report component reference that receives resize callbacks.</param>
    /// <param name="startClientY">Initial document pointer Y coordinate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StartSectionResize<TItem>( DotNetObjectReference<Report<TItem>> dotNetObjectReference, double startClientY )
    {
        await InvokeSafeVoidAsync( "startSectionResize", dotNetObjectReference, startClientY );
    }

    /// <summary>
    /// Stops any active document-level band resize listeners.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StopSectionResize()
    {
        await InvokeSafeVoidAsync( "stopSectionResize" );
    }

    /// <summary>
    /// Calculates client coordinates relative to an element.
    /// </summary>
    /// <param name="element">Element used as the coordinate origin.</param>
    /// <param name="clientX">Document client X coordinate.</param>
    /// <param name="clientY">Document client Y coordinate.</param>
    /// <returns>Element-local coordinates as a two-item array containing X and Y values.</returns>
    public virtual async ValueTask<double[]> GetElementOffset( ElementReference element, double clientX, double clientY )
    {
        return await InvokeSafeAsync<double[]>( "getElementOffset", element, clientX, clientY );
    }

    /// <summary>
    /// Suppresses the native browser drag image for draggable report tree nodes.
    /// </summary>
    /// <param name="element">Tree root element that owns draggable nodes.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask SuppressTreeNativeDragImage( ElementReference element )
    {
        await InvokeSafeVoidAsync( "suppressTreeNativeDragImage", element );
    }

    /// <summary>
    /// Clears native browser drag image suppression for a report tree.
    /// </summary>
    /// <param name="element">Tree root element that owns draggable nodes.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask ClearTreeNativeDragImage( ElementReference element )
    {
        await InvokeSafeVoidAsync( "clearTreeNativeDragImage", element );
    }

    /// <summary>
    /// Enables atomic editing behavior for expression tokens inside a report text editor.
    /// </summary>
    /// <param name="element">Text editor element that contains report expression tokens.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask ProtectTextExpressionTokens( ElementReference element )
    {
        await InvokeSafeVoidAsync( "protectTextExpressionTokens", element );
    }

    /// <summary>
    /// Disables atomic expression token editing behavior for a report text editor.
    /// </summary>
    /// <param name="element">Text editor element that contains report expression tokens.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask ClearTextExpressionTokenProtection( ElementReference element )
    {
        await InvokeSafeVoidAsync( "clearTextExpressionTokenProtection", element );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Reporting/blazorise.reporting.js?v={VersionProvider.Version}";

    #endregion
}