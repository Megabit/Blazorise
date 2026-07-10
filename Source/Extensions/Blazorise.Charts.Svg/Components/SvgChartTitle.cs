#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines the title of a native SVG chart.
/// </summary>
public class SvgChartTitle : SvgChartComponentBase
{
    #region Members

    private ComponentParameterInfo<bool> paramVisible;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Visible, out paramVisible );

        return base.SetParametersAsync( parameters );
    }

    protected override void Register()
    {
        Parent?.RegisterTitle( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterTitle( this );
    }

    internal bool ResolveVisible( bool fallback )
    {
        return paramVisible.GetValueOrDefault( fallback );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether the title and subtitle are visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines title options.
    /// </summary>
    [Parameter] public SvgChartTextOptions Title { get; set; }

    /// <summary>
    /// Defines subtitle options.
    /// </summary>
    [Parameter] public SvgChartTextOptions Subtitle { get; set; }

    #endregion
}