#region Using directives
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Annotation;

public partial class ChartAnnotation<TItem> : BaseComponent, IAsyncDisposable
{
    #region Members

    private const string PluginName = "DataAnnotation";

    #endregion

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && JSModule is not null )
        {
            //var datasetsChanged = parameters.TryGetValue<List<ChartDataLabelsDataset>>( nameof( Datasets ), out var paramDataset ) && !Datasets.AreEqual( paramDataset );
            var optionsChanged = parameters.TryGetValue<Dictionary<string, ChartAnnotationOptions>>( nameof( Options ), out var paramOptions ) && !Options.IsEqual( paramOptions );

            if ( /*datasetsChanged ||*/ optionsChanged )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSModule.AddAnnotationOptions( ParentChart.ElementId, Options );
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
            JSModule = new JSChartAnnotationModule( JSRuntime, VersionProvider );

            ExecuteAfterRender( async () =>
            {
                await JSModule.AddAnnotationOptions( ParentChart.ElementId, Options );
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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    protected JSChartAnnotationModule JSModule { get; private set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    /// <summary>
    /// Defines the options for an annotation.
    /// </summary>
    [Parameter] public Dictionary<string, ChartAnnotationOptions> Options { get; set; }
}
