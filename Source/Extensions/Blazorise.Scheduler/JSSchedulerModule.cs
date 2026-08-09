#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Manages pointer selection behavior for a scheduler in the browser.
/// </summary>
public class JSSchedulerModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Members

    private readonly Func<ElementReference> getElementRef;

    private readonly Func<string> getElementId;

    #endregion

    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    /// <param name="getElementRef">Function to get the element reference.</param>
    /// <param name="getElementId">Function to get the element id.</param>
    public JSSchedulerModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options,
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
    /// Attaches scheduler selection listeners and .NET callbacks.
    /// </summary>
    public virtual async ValueTask Initialize<TItem>( DotNetObjectReference<Scheduler<TItem>> dotNetObjectReference )
    {
        await InvokeSafeVoidAsync( "initialize", dotNetObjectReference, ElementRef, ElementId );
    }

    /// <summary>
    /// Detaches browser behavior from a scheduler element.
    /// </summary>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <summary>
    /// Marks the beginning of a scheduler range selection.
    /// </summary>
    public virtual async ValueTask SelectionStarted()
    {
        await InvokeSafeVoidAsync( "selectionStarted", ElementRef, ElementId );
    }

    /// <summary>
    /// Marks completion of the active scheduler range selection.
    /// </summary>
    public virtual async ValueTask SelectionEnded()
    {
        await InvokeSafeVoidAsync( "selectionEnded", ElementRef, ElementId );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the element reference.
    /// </summary>
    private ElementReference ElementRef => getElementRef.Invoke();

    /// <summary>
    /// Gets the element id.
    /// </summary>
    private string ElementId => getElementId.Invoke();

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Scheduler/scheduler.js?v={VersionProvider.Version}";

    #endregion
}