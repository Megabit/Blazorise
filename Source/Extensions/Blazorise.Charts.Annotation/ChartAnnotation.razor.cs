#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Provides the annotation capabilities to the supported chart types.
/// </summary>
/// <typeparam name="TItem">Data point type.</typeparam>
public partial class ChartAnnotation<TItem> : ChartPlugin<TItem, JSChartAnnotationModule>
{
    #region Methods

    /// <inheritdoc/>
    protected override JSChartAnnotationModule GetNewJsModule()
        => new( JSRuntime, VersionProvider, BlazoriseOptions );

    /// <inheritdoc/>
    protected override async Task InitializePluginByJsModule()
        => await JSModule.AddAnnotationOptions( ParentChart.ElementId, Options );

    /// <inheritdoc/>
    protected override bool InitPluginInParameterSet( ParameterView parameters )
        => parameters.TryGetValue<Dictionary<string, ChartAnnotationOptions>>( nameof( Options ), out var paramOptions ) && !Options.IsEqual( paramOptions );

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string Name => "DataAnnotation";

    /// <inheritdoc/>
    protected override JSChartAnnotationModule JSModule { get; set; }

    /// <summary>
    /// Defines the options for an annotation.
    /// </summary>
    [Parameter] public Dictionary<string, ChartAnnotationOptions> Options { get; set; }

    #endregion
}
