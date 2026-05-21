#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines data labels for a native SVG chart.
/// </summary>
public class SvgChartDataLabels : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterDataLabels( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterDataLabels( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether data labels are visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines whether data labels react to pointer and keyboard interactions.
    /// </summary>
    [Parameter] public bool Interactive { get; set; } = true;

    /// <summary>
    /// Defines the data label position.
    /// </summary>
    [Parameter] public SvgChartDataLabelPosition Position { get; set; } = SvgChartDataLabelPosition.Auto;

    /// <summary>
    /// Defines the label offset from the point anchor.
    /// </summary>
    [Parameter] public double Offset { get; set; } = 6;

    /// <summary>
    /// Defines the label opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 1;

    /// <summary>
    /// Defines data label font options.
    /// </summary>
    [Parameter] public SvgChartFontOptions Font { get; set; }

    /// <summary>
    /// Defines the label padding.
    /// </summary>
    [Parameter] public SvgChartSpacing Padding { get; set; }

    /// <summary>
    /// Defines the label background color.
    /// </summary>
    [Parameter] public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the label border options.
    /// </summary>
    [Parameter] public SvgChartBorderOptions Border { get; set; }

    /// <summary>
    /// Defines a callback used to format label text.
    /// </summary>
    [Parameter] public Func<SvgChartDataLabelContext, string> Formatter { get; set; }

    /// <summary>
    /// Occurs when a data label is clicked.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when a data label is hovered.
    /// </summary>
    [Parameter] public EventCallback<SvgChartPointEventArgs> Hovered { get; set; }

    #endregion
}