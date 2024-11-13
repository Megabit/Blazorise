#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.DataLabels;

/// <summary>
/// Highly customizable Chart.js plugin that displays labels on data for any type of charts.
/// </summary>
/// <typeparam name="TItem">Generic dataset value type.</typeparam>
public partial class ChartDataLabels<TItem> : BaseComponent, IAsyncDisposable
{
    #region Members

    private const string PluginName = "DataLabels";

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && JSModule is not null )
        {
            var datasetsChanged = parameters.TryGetValue<List<ChartDataLabelsDataset>>( nameof( Datasets ), out var paramDataset ) && !Datasets.AreEqual( paramDataset );
            var optionsChanged = parameters.TryGetValue<ChartDataLabelsOptions>( nameof( Options ), out var paramOptions ) && !Options.IsEqual( paramOptions );

            if ( datasetsChanged || optionsChanged )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSModule.SetDataLabels( ParentChart.ElementId, Datasets, Options );
                } );
            }
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( ParentChart is not null )
        {
            ParentChart.Initialized += OnParentChartInitialized;

            ParentChart.NotifyPluginInitialized( PluginName );
        }

        return base.OnInitializedAsync();
    }

    private async void OnParentChartInitialized( object sender, EventArgs e )
    {
        if ( JSModule == null )
        {
            JSModule = new JSChartDataLabelsModule( JSRuntime, VersionProvider, BlazoriseOptions );

            ExecuteAfterRender( async () =>
            {
                await JSModule.SetDataLabels( ParentChart.ElementId, Datasets, Options );
            } );

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDisposeAsync();

            if ( ParentChart is not null )
            {
                ParentChart.Initialized -= OnParentChartInitialized;

                ParentChart.NotifyPluginRemoved( PluginName );
            }
        }

        await base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// JS module that handles the calls to the datalabels JS interop.
    /// </summary>
    protected JSChartDataLabelsModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// List of datalabels to apply to the parent chart.
    /// </summary>
    [Parameter] public List<ChartDataLabelsDataset> Datasets { get; set; }

    /// <summary>
    /// Datalabels that will be set to the options instead of to the datasets.
    /// </summary>
    [Parameter] public ChartDataLabelsOptions Options { get; set; }

    /// <summary>
    /// Parent chart component.
    /// </summary>
    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    #endregion
}