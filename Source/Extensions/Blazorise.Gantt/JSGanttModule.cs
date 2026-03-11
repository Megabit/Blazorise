#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Provides JavaScript interop functionality for managing Gantt chart components in Blazor applications.
/// </summary>
/// <remarks>This module enables initialization, destruction, and interaction with Gantt chart elements through
/// JavaScript. It is intended to be used as part of the Blazorise Gantt component infrastructure and implements
/// asynchronous operations for proper resource management. The module should be instantiated with valid element
/// reference and identifier providers to ensure correct operation.</remarks>
public class JSGanttModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Members

    private readonly Func<ElementReference> getElementRef;

    private readonly Func<string> getElementId;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the JSGanttModule class with the specified JavaScript runtime, version provider,
    /// options, and element reference accessors.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime used to execute interop calls.</param>
    /// <param name="versionProvider">The provider used to supply version information for the module.</param>
    /// <param name="options">The configuration options for the Blazorise module.</param>
    /// <param name="getElementRef">A function that returns the ElementReference for the target DOM element.</param>
    /// <param name="getElementId">A function that returns the identifier of the target DOM element.</param>
    public JSGanttModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options,
        Func<ElementReference> getElementRef,
        Func<string> getElementId )
        : base( jsRuntime, versionProvider, options )
    {
        this.getElementRef = getElementRef;
        this.getElementId = getElementId;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the Gantt component by establishing a connection with the specified .NET object reference.
    /// </summary>
    /// <remarks>This method should be called before interacting with the Gantt component to ensure proper
    /// setup. The initialization process is asynchronous and must complete before the component is fully
    /// operational.</remarks>
    /// <typeparam name="TItem">The type of items managed by the Gantt component.</typeparam>
    /// <param name="dotNetObjectReference">A reference to the .NET Gantt object to be initialized. Cannot be null.</param>
    /// <returns>A ValueTask representing the asynchronous initialization operation.</returns>
    public virtual async ValueTask Initialize<TItem>( DotNetObjectReference<Gantt<TItem>> dotNetObjectReference )
    {
        await InvokeSafeVoidAsync( "initialize", dotNetObjectReference, ElementRef, ElementId );
    }

    /// <summary>
    /// Destroys the specified element, releasing any associated resources or event handlers.
    /// </summary>
    /// <remarks>Call this method when the element is no longer needed to ensure proper cleanup. This
    /// operation is asynchronous and should be awaited to guarantee completion before proceeding.</remarks>
    /// <param name="elementRef">A reference to the element to be destroyed. Must represent a valid DOM element.</param>
    /// <param name="elementId">The unique identifier of the element to destroy. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous destroy operation.</returns>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <summary>
    /// Initiates the drag operation for the bar element at the specified horizontal client coordinate.
    /// </summary>
    /// <param name="startClientX">The horizontal client coordinate, in pixels, where the drag operation begins. Must be a valid screen coordinate.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the drag initiation logic has
    /// finished executing.</returns>
    public virtual async ValueTask BarDragStarted( double startClientX )
    {
        await InvokeSafeVoidAsync( "barDragStarted", ElementRef, ElementId, startClientX );
    }

    /// <summary>
    /// Signals that a bar drag operation has completed and performs any necessary cleanup or finalization.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async ValueTask BarDragEnded()
    {
        await InvokeSafeVoidAsync( "barDragEnded", ElementRef, ElementId );
    }

    #endregion

    #region Properties

    private ElementReference ElementRef => getElementRef.Invoke();

    private string ElementId => getElementId.Invoke();

    /// <summary>
    /// Gets the relative file path to the JavaScript module used by the Gantt component.
    /// </summary>
    public override string ModuleFileName => $"./_content/Blazorise.Gantt/gantt.js?v={VersionProvider.Version}";

    #endregion
}