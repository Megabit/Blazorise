#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Default implementation of the video JS module.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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

    public virtual async ValueTask Initialize<TItem>( DotNetObjectReference<Scheduler<TItem>> dotNetObjectReference )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, ElementRef, ElementId );
    }

    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
    }

    public virtual async ValueTask SelectionStarted()
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "selectionStarted", ElementRef, ElementId );
    }

    public virtual async ValueTask SelectionEnded()
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "selectionEnded", ElementRef, ElementId );
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member