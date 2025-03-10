#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Provides the annotation capabilities to the supported chart types.
/// </summary>
/// <typeparam name="TItem">Data point type.</typeparam>
public partial class ChartZoom<TItem> : BaseComponent, IAsyncDisposable
{
    #region Members

    private const string PluginName = "Zoom";

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered && JSModule is not null )
        {
            var optionsChanged = parameters.TryGetValue<ChartZoomPluginOptions>( nameof( Options ), out var paramOptions ) && !Options.IsEqual( paramOptions );

            if ( optionsChanged )
            {
                ExecuteAfterRender( async () =>
                {
                    await JSModule.AddZoom( ParentChart.ElementId, Options );
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
            JSModule = new JSChartZoomModule( JSRuntime, VersionProvider, BlazoriseOptions );

            ExecuteAfterRender( async () =>
            {
                await JSModule.AddZoom( ParentChart.ElementId, Options );
            } );

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            if(JSModule is not null)
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

    protected JSChartZoomModule JSModule { get; private set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    /// <summary>
    /// Defines the options for an annotation.
    /// </summary>
    [Parameter] public ChartZoomPluginOptions Options { get; set; }

    #endregion
}
