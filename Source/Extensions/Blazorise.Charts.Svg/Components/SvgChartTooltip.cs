#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines tooltip behavior for a native SVG chart.
/// </summary>
public class SvgChartTooltip : SvgChartComponentBase
{
    #region Members

    private ComponentParameterInfo<bool> paramEnabled;

    private ComponentParameterInfo<SvgChartInteractionMode> paramInteractionMode;

    private ComponentParameterInfo<Func<SvgChartTooltipContext, string>> paramFormatter;

    private ComponentParameterInfo<RenderFragment<SvgChartTooltipContext>> paramTemplate;

    private ComponentParameterInfo<double> paramWidth;

    private ComponentParameterInfo<double> paramHeight;

    private ComponentParameterInfo<double> paramOffsetX;

    private ComponentParameterInfo<double> paramOffsetY;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        parameters.TryGetParameter( Enabled, out paramEnabled );
        parameters.TryGetParameter( InteractionMode, out paramInteractionMode );
        parameters.TryGetParameter( Formatter, out paramFormatter );
        parameters.TryGetParameter( Template, out paramTemplate );
        parameters.TryGetParameter( Width, out paramWidth );
        parameters.TryGetParameter( Height, out paramHeight );
        parameters.TryGetParameter( OffsetX, out paramOffsetX );
        parameters.TryGetParameter( OffsetY, out paramOffsetY );

        return base.SetParametersAsync( parameters );
    }

    protected override void Register()
    {
        Parent?.RegisterTooltip( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterTooltip( this );
    }

    internal SvgChartTooltipOptions ResolveOptions( SvgChartTooltipOptions fallback )
    {
        fallback ??= new();

        return new()
        {
            Enabled = paramEnabled.GetValueOrDefault( fallback.Enabled ),
            InteractionMode = paramInteractionMode.GetValueOrDefault( fallback.InteractionMode ),
            Formatter = paramFormatter.GetValueOrDefault( fallback.Formatter ),
            Template = paramTemplate.GetValueOrDefault( fallback.Template ),
            Width = paramWidth.GetValueOrDefault( fallback.Width ),
            Height = paramHeight.GetValueOrDefault( fallback.Height ),
            OffsetX = paramOffsetX.GetValueOrDefault( fallback.OffsetX ),
            OffsetY = paramOffsetY.GetValueOrDefault( fallback.OffsetY )
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether SVG chart tooltips are shown for points.
    /// </summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// Defines how related points are resolved for tooltip content.
    /// </summary>
    [Parameter] public SvgChartInteractionMode InteractionMode { get; set; } = SvgChartInteractionMode.Nearest;

    /// <summary>
    /// Defines a callback used to format default tooltip text.
    /// </summary>
    [Parameter] public Func<SvgChartTooltipContext, string> Formatter { get; set; }

    /// <summary>
    /// Defines custom tooltip content.
    /// </summary>
    [Parameter] public RenderFragment<SvgChartTooltipContext> Template { get; set; }

    /// <summary>
    /// Defines the tooltip width in SVG viewport units.
    /// </summary>
    [Parameter] public double Width { get; set; } = 180;

    /// <summary>
    /// Defines the tooltip height in SVG viewport units.
    /// </summary>
    [Parameter] public double Height { get; set; } = 56;

    /// <summary>
    /// Defines the horizontal tooltip offset from the point anchor.
    /// </summary>
    [Parameter] public double OffsetX { get; set; } = 8;

    /// <summary>
    /// Defines the vertical tooltip offset from the point anchor.
    /// </summary>
    [Parameter] public double OffsetY { get; set; } = 8;

    #endregion
}