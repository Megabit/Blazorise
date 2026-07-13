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
    /// <typeparam name="TComponent">Component type that receives resize callbacks.</typeparam>
    /// <param name="dotNetObjectReference">Component reference that receives resize callbacks.</param>
    /// <param name="startClientY">Initial document pointer Y coordinate.</param>
    /// <param name="pointerId">The starting pointer identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StartSectionResize<TComponent>( DotNetObjectReference<TComponent> dotNetObjectReference, double startClientY, long pointerId )
        where TComponent : class
    {
        await InvokeSafeVoidAsync( "startSectionResize", dotNetObjectReference, startClientY, pointerId );
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
    /// Starts document-level element resizing for the active report designer.
    /// </summary>
    /// <typeparam name="TComponent">Component type that receives resize callbacks.</typeparam>
    /// <param name="dotNetObjectReference">Component reference that receives resize callbacks.</param>
    /// <param name="startClientX">Initial document pointer X coordinate.</param>
    /// <param name="startClientY">Initial document pointer Y coordinate.</param>
    /// <param name="pointerId">The starting pointer identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StartElementResize<TComponent>( DotNetObjectReference<TComponent> dotNetObjectReference, double startClientX, double startClientY, long pointerId )
        where TComponent : class
    {
        await InvokeSafeVoidAsync( "startElementResize", dotNetObjectReference, startClientX, startClientY, pointerId );
    }

    /// <summary>
    /// Stops any active document-level element resize listeners.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StopElementResize()
    {
        await InvokeSafeVoidAsync( "stopElementResize" );
    }

    /// <summary>
    /// Starts document-level element movement for the active report designer.
    /// </summary>
    /// <typeparam name="TComponent">Component type that receives movement callbacks.</typeparam>
    /// <param name="pageElement">Designer page used to resolve the band beneath the pointer.</param>
    /// <param name="dotNetObjectReference">Component reference that receives movement callbacks.</param>
    /// <param name="startClientX">Initial document pointer X coordinate.</param>
    /// <param name="startClientY">Initial document pointer Y coordinate.</param>
    /// <param name="pointerId">The starting pointer identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StartElementDrag<TComponent>( ElementReference pageElement, DotNetObjectReference<TComponent> dotNetObjectReference, double startClientX, double startClientY, long pointerId )
        where TComponent : class
    {
        await InvokeSafeVoidAsync( "startElementDrag", pageElement, dotNetObjectReference, startClientX, startClientY, pointerId );
    }

    /// <summary>
    /// Stops any active document-level element movement listeners.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StopElementDrag()
    {
        await InvokeSafeVoidAsync( "stopElementDrag" );
    }

    /// <summary>
    /// Starts document-level designer keyboard shortcut handling for the active report designer.
    /// </summary>
    /// <typeparam name="TComponent">Component type that receives shortcut callbacks.</typeparam>
    /// <param name="element">Designer root element that scopes shortcut activation.</param>
    /// <param name="dotNetObjectReference">Component reference that receives shortcut callbacks.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StartDesignerKeyboardShortcuts<TComponent>( ElementReference element, DotNetObjectReference<TComponent> dotNetObjectReference )
        where TComponent : class
    {
        await InvokeSafeVoidAsync( "startDesignerKeyboardShortcuts", element, dotNetObjectReference );
    }

    /// <summary>
    /// Stops document-level designer keyboard shortcut handling for a report designer.
    /// </summary>
    /// <param name="element">Designer root element that owns the shortcut listener.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask StopDesignerKeyboardShortcuts( ElementReference element )
    {
        await InvokeSafeVoidAsync( "stopDesignerKeyboardShortcuts", element );
    }

    /// <summary>
    /// Calculates client coordinates relative to an element.
    /// </summary>
    /// <param name="element">Element used as the coordinate origin.</param>
    /// <param name="clientX">Document client X coordinate.</param>
    /// <param name="clientY">Document client Y coordinate.</param>
    /// <returns>Element-local coordinates as a two-item array containing X and Y values.</returns>
    public virtual ValueTask<double[]> GetElementOffset( ElementReference element, double clientX, double clientY )
        => InvokeSafeAsync<double[]>( "getElementOffset", element, clientX, clientY );

    /// <summary>
    /// Gets the current scroll position of an element.
    /// </summary>
    /// <param name="element">Element to inspect.</param>
    /// <returns>A two-item array containing horizontal and vertical scroll offsets.</returns>
    public virtual ValueTask<double[]> GetScrollPosition( ElementReference element )
        => InvokeSafeAsync<double[]>( "getScrollPosition", element );

    /// <summary>
    /// Sets the scroll position of an element.
    /// </summary>
    /// <param name="element">Element to scroll.</param>
    /// <param name="left">Horizontal scroll offset.</param>
    /// <param name="top">Vertical scroll offset.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask SetScrollPosition( ElementReference element, double left, double top )
    {
        await InvokeSafeVoidAsync( "setScrollPosition", element, left, top );
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
    /// Scrolls the selected report tree node into view.
    /// </summary>
    /// <param name="element">Tree root element that contains the node.</param>
    /// <param name="nodeKey">Key of the node to scroll into view.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask ScrollTreeNodeIntoView( ElementReference element, string nodeKey )
    {
        await InvokeSafeVoidAsync( "scrollTreeNodeIntoView", element, nodeKey );
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

    /// <summary>
    /// Downloads generated file content in the browser.
    /// </summary>
    /// <param name="fileName">Suggested download file name.</param>
    /// <param name="contentType">Generated file content type.</param>
    /// <param name="content">Generated file bytes.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask DownloadFile( string fileName, string contentType, byte[] content )
    {
        await InvokeSafeVoidAsync( "downloadFile", fileName, contentType, content );
    }

    /// <summary>
    /// Updates the designer selection overlay without forcing a Blazor render.
    /// </summary>
    /// <param name="pageElement">Designer page element that owns the overlay.</param>
    /// <param name="x">Left coordinate in CSS pixels.</param>
    /// <param name="y">Top coordinate in CSS pixels.</param>
    /// <param name="width">Overlay width in CSS pixels.</param>
    /// <param name="height">Overlay height in CSS pixels.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask UpdateDesignerSelectionOverlay( ElementReference pageElement, double x, double y, double width, double height )
    {
        await InvokeSafeVoidAsync( "updateDesignerSelectionOverlay", pageElement, x, y, width, height );
    }

    /// <summary>
    /// Updates the designer drag overlay without forcing a Blazor render.
    /// </summary>
    /// <param name="pageElement">Designer page element that owns the overlay.</param>
    /// <param name="elementType">Report element type represented by the overlay.</param>
    /// <param name="text">Preview text.</param>
    /// <param name="x">Left coordinate in CSS pixels.</param>
    /// <param name="y">Top coordinate in CSS pixels.</param>
    /// <param name="width">Overlay width in CSS pixels.</param>
    /// <param name="height">Overlay height in CSS pixels.</param>
    /// <param name="colliding">Indicates that the preview overlaps another element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask UpdateDesignerDragOverlay( ElementReference pageElement, string elementType, string text, double x, double y, double width, double height, bool colliding )
    {
        await InvokeSafeVoidAsync( "updateDesignerDragOverlay", pageElement, elementType, text, x, y, width, height, colliding );
    }

    /// <summary>
    /// Clears designer interaction overlays owned by the page.
    /// </summary>
    /// <param name="pageElement">Designer page element that owns the overlay.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask ClearDesignerInteractionOverlays( ElementReference pageElement )
    {
        await InvokeSafeVoidAsync( "clearDesignerInteractionOverlays", pageElement );
    }

    /// <summary>
    /// Updates a designer section resize preview without forcing a Blazor render.
    /// </summary>
    /// <param name="pageElement">Designer page element that owns the section.</param>
    /// <param name="sectionId">Section identifier.</param>
    /// <param name="height">Section height in CSS pixels.</param>
    /// <param name="markerY">Vertical ruler marker top in CSS pixels.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask UpdateDesignerSectionResizePreview( ElementReference pageElement, string sectionId, double height, double markerY )
    {
        await InvokeSafeVoidAsync( "updateDesignerSectionResizePreview", pageElement, sectionId, height, markerY );
    }

    /// <summary>
    /// Restores a pending designer section resize preview.
    /// </summary>
    /// <param name="pageElement">Designer page element that owns the section.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask ClearDesignerSectionResizePreview( ElementReference pageElement )
    {
        await InvokeSafeVoidAsync( "clearDesignerSectionResizePreview", pageElement );
    }

    /// <summary>
    /// Keeps the pending designer section resize preview until the next Blazor render commits the model value.
    /// </summary>
    /// <param name="pageElement">Designer page element that owns the section.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask CommitDesignerSectionResizePreview( ElementReference pageElement )
    {
        await InvokeSafeVoidAsync( "commitDesignerSectionResizePreview", pageElement );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Reporting/blazorise.reporting.js?v={VersionProvider.Version}";

    #endregion
}