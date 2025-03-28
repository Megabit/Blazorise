#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline;

/// <summary>
/// This plugin (computes and) draws a linear trend line in the chart.
/// This plugin works with @ref and call of the AddTrendLineOptions method after
/// dataset has been added and chart.Update has been called
/// </summary>
/// <typeparam name="TItem">Data point type.</typeparam>
public partial class ChartTrendline<TItem> : BaseComponent, IChartTrendline, IAsyncDisposable
// doesn't inherit from ChartPlugin as the other plugins, since it's called from users code and doesn't need the
// management synchronization with the ParentChart
{
    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSChartTrendlineModule( JSRuntime, VersionProvider, BlazoriseOptions );

        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Adds the trendline options to the chart.
    /// </summary>
    /// <param name="trendlineData">The trendline data to be added to the chart.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddTrendLineOptions( List<ChartTrendlineData> trendlineData )
    {
        if ( !Rendered )
            return;

        await JSModule.AddTrendlineOptions( ParentChart.ElementId, trendlineData );
    }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected JSChartTrendlineModule JSModule { get; private set; }

    /// <summary>
    /// Represents a cascading parameter that provides access to the parent chart of type <see cref="BaseChart{TItem}"/>.
    /// </summary>
    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    /// <summary>
    /// Injects an instance of IJSRuntime for JavaScript interop in a Blazor component.
    /// </summary>
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Injects an instance of IVersionProvider for version management.
    /// </summary>
    [Inject] protected IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }
}