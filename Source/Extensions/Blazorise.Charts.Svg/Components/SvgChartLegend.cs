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

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Visible, out paramVisible );
        parameters.TryGetParameter( Position, out paramPosition );

        return base.SetParametersAsync( parameters );
    }

    protected override void Register()
    {
        Parent?.RegisterLegend( this );
        SetRegisteredParent();
    }

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
            Position = paramPosition.GetValueOrDefault( fallback.Position )
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether the legend is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the legend position.
    /// </summary>
    [Parameter] public SvgChartLegendPosition Position { get; set; } = SvgChartLegendPosition.Bottom;

    #endregion
}