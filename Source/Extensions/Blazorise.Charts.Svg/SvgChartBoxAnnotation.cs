#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a box annotation for a native SVG chart.
/// </summary>
public class SvgChartBoxAnnotation : SvgChartComponentBase
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
    /// Defines the annotation rendering order among box annotations. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    /// <summary>
    /// Defines the annotation start X value. For category charts, this is the category index where <c>-0.5</c> represents the left plot edge.
    /// </summary>
    [Parameter] public double? XMin { get; set; }

    /// <summary>
    /// Defines the annotation end X value. For category charts, this is the category index where <c>-0.5</c> represents the left plot edge.
    /// </summary>
    [Parameter] public double? XMax { get; set; }

    /// <summary>
    /// Defines the annotation start Y value.
    /// </summary>
    [Parameter] public double? YMin { get; set; }

    /// <summary>
    /// Defines the annotation end Y value.
    /// </summary>
    [Parameter] public double? YMax { get; set; }

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
    [Parameter] public double Opacity { get; set; } = 0.16;

    /// <summary>
    /// Defines annotation label options.
    /// </summary>
    [Parameter] public SvgChartAnnotationLabelOptions Label { get; set; }

    #endregion
}