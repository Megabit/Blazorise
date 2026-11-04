#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the legend of a native SVG chart.
/// </summary>
public class SvgChartLegend : SvgChartComponentBase
{
    #region Members

    private ComponentParameterInfo<bool> paramVisible;

    private ComponentParameterInfo<SvgChartLegendPosition> paramPosition;

    private ComponentParameterInfo<SvgChartLegendLabelsOptions> paramLabels;

    private ComponentParameterInfo<SvgChartSpacing> paramPadding;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Visible, out paramVisible );
        parameters.TryGetParameter( Position, out paramPosition );
        parameters.TryGetParameter( Labels, out paramLabels );
        parameters.TryGetParameter( Padding, out paramPadding );

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override void Register()
    {
        Parent?.RegisterLegend( this );
        SetRegisteredParent();
    }

    /// <inheritdoc />
    protected override void Unregister()
    {
        RegisteredParent?.UnregisterLegend( this );
    }

    internal SvgChartLegendOptions ResolveOptions( SvgChartLegendOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Visible = paramVisible.GetValueOrDefault( fallback.Visible ),
            Position = paramPosition.GetValueOrDefault( fallback.Position ),
            Labels = paramLabels.Defined && paramLabels.Value is null
                ? null
                : SvgChartOptionsMapper.CreateLegendLabelsOptions( fallback.Labels, paramLabels.GetValueOrDefault( null ) ),
            Padding = paramPadding.GetValueOrDefault( fallback.Padding ),
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether the legend is visible.
    /// </summary>
    /// <remarks>When omitted, inherits the corresponding chart option.</remarks>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the legend position.
    /// </summary>
    /// <remarks>When omitted, inherits the corresponding chart option.</remarks>
    [Parameter] public SvgChartLegendPosition Position { get; set; } = SvgChartLegendPosition.Bottom;

    /// <summary>
    /// Defines legend label styling, color boxes, and item spacing.
    /// </summary>
    /// <remarks>Unspecified label settings inherit the corresponding chart options.</remarks>
    [Parameter] public SvgChartLegendLabelsOptions Labels { get; set; }

    /// <summary>
    /// Defines space around the legend, in SVG units.
    /// </summary>
    /// <remarks>When omitted, inherits the corresponding chart option.</remarks>
    [Parameter] public SvgChartSpacing Padding { get; set; }

    #endregion
}