#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Provides the annotation capabilities to the supported chart types.
/// </summary>
/// <typeparam name="TItem">Data point type.</typeparam>
public partial class ChartZoom<TItem> : ChartPlugin<TItem, JSChartZoomModule>
{
    #region Methods

    /// <inheritdoc/>
    protected override JSChartZoomModule GetNewJsModule()
        => new( JSRuntime, VersionProvider, BlazoriseOptions );

    /// <inheritdoc/>
    protected override async Task InitializePluginByJsModule()
        => await JSModule.AddZoom( ParentChart.ElementId, Options );

    /// <inheritdoc/>
    protected override bool InitPluginInParameterSet( ParameterView parameters )
        => parameters.TryGetValue<ChartZoomPluginOptions>( nameof( Options ), out var paramOptions ) && !Options.IsEqual( paramOptions );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string Name => "Zoom";

    /// <inheritdoc/>
    protected override JSChartZoomModule JSModule { get; set; }

    /// <summary>
    /// Defines the options for an annotation.
    /// </summary>
    [Parameter] public ChartZoomPluginOptions Options { get; set; }

    #endregion
}
