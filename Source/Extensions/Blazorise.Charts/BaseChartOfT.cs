#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Base class for all chart types.
/// </summary>
/// <typeparam name="TItem">Generic dataset value type.</typeparam>
public class BaseChart<TItem> : BaseComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Collection of plugins within the chart. Plugins are notified about chart initialization later.
    /// They cannot be properly initialized before the chart itself is fully initialized.
    /// </summary>
    private readonly List<ChartPlugin> chartPlugins = [];

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSChartModule( JSRuntime, VersionProvider, BlazoriseOptions );

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( DotNetObjectRef != null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Chart() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Notifies the chart and its plugins that it was properly initialized.
    /// This is called after chart render and initialization.
    /// That time, the plugins are already in the list.
    /// </summary>
    protected async Task NotifyInitialized()
    {
        var tasks = chartPlugins.Select( handler => handler.OnParentChartInitialized() );
        await Task.WhenAll( tasks );
    }

    /// <summary>
    /// Notifies the chart that it contains the plugin.
    /// </summary>
    /// <param name="plugin">Plugin name that is placed inside the chart.</param>
    public void NotifyPluginInitialized( ChartPlugin plugin )
    {
        chartPlugins.Add( plugin );
    }

    /// <summary>
    /// Notifies the chart that it should remove the plugin. Freeing the reference. 
    /// </summary>
    /// <param name="plugin">Plugin that is placed inside the chart.</param>
    public void NotifyPluginRemoved( ChartPlugin plugin )
    {
        chartPlugins.Remove( plugin );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Holds a reference to a DotNetObject of type ChartAdapter. It is used for interop between .NET and JavaScript.
    /// </summary>
    protected DotNetObjectReference<ChartAdapter> DotNetObjectRef { get; set; }

    /// <summary>
    /// Represents the JavaScript module for the current component.
    /// </summary>
    protected JSChartModule JSModule { get; private set; }

    /// <summary>
    /// Gets the list of registered plugins inside of this chart.
    /// </summary>
    protected IReadOnlyList<string> PluginNames => chartPlugins.Select( x => x.Name ).ToList();

    /// <summary>
    /// Injects an instance of IJSRuntime for JavaScript interop in a Blazor component.
    /// </summary>
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Injects an instance of IVersionProvider for version management.
    /// </summary>
    [Inject] protected IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Injects BlazoriseOptions for configuration. It allows access to Blazorise settings within the class.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Defines the chart data.
    /// </summary>
    [Parameter] public ChartData<TItem> Data { get; set; }

    /// <summary>
    /// Raised when clicked on data point. 
    /// </summary>
    [Parameter] public EventCallback<ChartMouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Raised when hovered over data point.
    /// </summary>
    [Parameter] public EventCallback<ChartMouseEventArgs> Hovered { get; set; }

    /// <summary>
    /// Raised when mouse leaves the chart area.
    /// </summary>
    [Parameter] public EventCallback<ChartMouseEventArgs> MouseOut { get; set; }

    /// <summary>
    /// Specifies the content to render inside this <see cref="BaseChart{TItem}"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}