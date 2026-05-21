#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a point annotation for a native SVG chart.
/// </summary>
public class SvgChartPointAnnotation : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterAnnotation( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterAnnotation( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether the annotation is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the annotation name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Defines the value axis identifier used by this annotation.
    /// </summary>
    [Parameter] public string ValueAxisId { get; set; }

    /// <summary>
    /// Defines the annotation rendering order among point annotations. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    /// <summary>
    /// Defines the annotation X value. For category charts, this is the category index.
    /// </summary>
    [Parameter] public double? X { get; set; }

    /// <summary>
    /// Defines the annotation Y value.
    /// </summary>
    [Parameter] public double? Y { get; set; }

    /// <summary>
    /// Defines the annotation point radius.
    /// </summary>
    [Parameter] public double Radius { get; set; } = 5;

    /// <summary>
    /// Defines the annotation background color.
    /// </summary>
    [Parameter] public Color BackgroundColor { get; set; }

    /// <summary>
    /// Defines the annotation border options.
    /// </summary>
    [Parameter] public SvgChartBorderOptions Border { get; set; }

    /// <summary>
    /// Defines the annotation opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 1;

    /// <summary>
    /// Defines annotation label options.
    /// </summary>
    [Parameter] public SvgChartAnnotationLabelOptions Label { get; set; }

    #endregion
}