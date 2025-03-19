#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline;

public interface IChartTrendline
{
    Task AddTrendLineOptions( List<ChartTrendlineData> trendlineData );
}

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
    protected override Task OnInitializedAsync()
    {
        JSModule ??= new JSChartTrendlineModule(JSRuntime, VersionProvider, BlazoriseOptions);

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

    protected JSChartTrendlineModule JSModule { get; private set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }
}