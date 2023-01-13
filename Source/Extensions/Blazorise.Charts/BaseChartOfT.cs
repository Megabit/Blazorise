﻿#region Using directives
using System;
using System.Collections.Generic;
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
    /// Occures after the chart has initialized successfully.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// List of registered plugins for this chart.
    /// </summary>
    private readonly List<string> pluginNames = new();

    #endregion

    #region Methods

    protected override Task OnInitializedAsync()
    {
        if ( JSModule == null )
        {
            JSModule = new JSChartModule( JSRuntime, VersionProvider );
        }

        return base.OnInitializedAsync();
    }

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

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Chart() );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Notifies the chart that it is being properly initialized.
    /// </summary>
    protected void NotifyInitialized()
    {
        Initialized?.Invoke( this, EventArgs.Empty );
    }

    /// <summary>
    /// Notifies the chart that it contains the plugin.
    /// </summary>
    /// <param name="pluginName">Plugin name that is placed inside of the chart.</param>
    public void NotifyPluginInitialized( string pluginName )
    {
        if ( !pluginNames.Contains( pluginName ) )
        {
            pluginNames.Add( pluginName );
        }
    }

    /// <summary>
    /// Notifies the chart that it should remove the plugin.
    /// </summary>
    /// <param name="pluginName">Plugin name that is placed inside of the chart.</param>
    public void NotifyPluginRemoved( string pluginName )
    {
        if ( pluginNames.Contains( pluginName ) )
        {
            pluginNames.Remove( pluginName );
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    protected DotNetObjectReference<ChartAdapter> DotNetObjectRef { get; set; }

    protected JSChartModule JSModule { get; private set; }

    /// <summary>
    /// Gets the list of registered plugins inside of this chart.
    /// </summary>
    protected IReadOnlyList<string> PluginNames => pluginNames;

    [Inject] protected IJSRuntime JSRuntime { get; set; }

    [Inject] protected IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Defines the chart data.
    /// </summary>
    [Parameter] public ChartData<TItem> Data { get; set; }

    [Parameter] public EventCallback<ChartMouseEventArgs> Clicked { get; set; }

    [Parameter] public EventCallback<ChartMouseEventArgs> Hovered { get; set; }

    [Parameter] public EventCallback<ChartMouseEventArgs> MouseOut { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}