#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// JavaScript module wrapper for Gantt component interactions.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class JSGanttModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Members

    private readonly Func<ElementReference> getElementRef;

    private readonly Func<string> getElementId;

    #endregion

    #region Constructors

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

    public virtual async ValueTask Initialize<TItem>( DotNetObjectReference<Gantt<TItem>> dotNetObjectReference )
    {
        await InvokeSafeVoidAsync( "initialize", dotNetObjectReference, ElementRef, ElementId );
    }

    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    public virtual async ValueTask BarDragStarted( double startClientX )
    {
        await InvokeSafeVoidAsync( "barDragStarted", ElementRef, ElementId, startClientX );
    }

    public virtual async ValueTask BarDragEnded()
    {
        await InvokeSafeVoidAsync( "barDragEnded", ElementRef, ElementId );
    }

    #endregion

    #region Properties

    private ElementReference ElementRef => getElementRef.Invoke();

    private string ElementId => getElementId.Invoke();

    public override string ModuleFileName => $"./_content/Blazorise.Gantt/gantt.js?v={VersionProvider.Version}";

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member