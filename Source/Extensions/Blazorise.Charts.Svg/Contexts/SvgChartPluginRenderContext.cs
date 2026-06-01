#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Provides rendering information and projection helpers for native SVG chart plugins.
/// </summary>
public sealed class SvgChartPluginRenderContext
{
    #region Members

    private readonly Func<double, string, double> projectCategory;

    private readonly Func<double, string, double> projectCategoryBoundary;

    private readonly Func<double, string, double> projectX;

    private readonly Func<double, string, double> projectValueX;

    private readonly Func<double, string, double> projectY;

    private readonly Func<double?, double, double> projectAnnotationX;

    private readonly Func<double?, string, double, double> projectAnnotationY;

    private readonly Func<string, int, bool> isDataPointHidden;

    private readonly Func<SvgChartPointEventArgs, string, Task> pointClicked;

    private readonly Func<SvgChartPointEventArgs, string, Task> pointHovered;

    private readonly Func<SvgChartPointEventArgs, string, Task> dataLabelClicked;

    private readonly Func<SvgChartPointEventArgs, string, Task> dataLabelHovered;

    private readonly Action pointLeft;

    private readonly Action<SvgChartPointEventArgs, string, bool> showTooltip;

    private readonly Func<Task> refresh;

    #endregion

    #region Constructors

    internal SvgChartPluginRenderContext(
        SvgChartType type,
        SvgChartOptions options,
        SvgChartPluginPlotArea plotArea,
        IReadOnlyList<object> labels,
        IReadOnlyList<SvgChartPluginSeries> series,
        bool radial,
        double valueMin,
        double valueMax,
        int categorySlotCount,
        SvgChartAxisScaleKind categoryScaleKind,
        object eventReceiver,
        Func<double, string, double> projectCategory,
        Func<double, string, double> projectCategoryBoundary,
        Func<double, string, double> projectX,
        Func<double, string, double> projectValueX,
        Func<double, string, double> projectY,
        Func<double?, double, double> projectAnnotationX,
        Func<double?, string, double, double> projectAnnotationY,
        Func<string, int, bool> isDataPointHidden,
        Func<SvgChartPointEventArgs, string, Task> pointClicked,
        Func<SvgChartPointEventArgs, string, Task> pointHovered,
        Func<SvgChartPointEventArgs, string, Task> dataLabelClicked,
        Func<SvgChartPointEventArgs, string, Task> dataLabelHovered,
        Action pointLeft,
        Action<SvgChartPointEventArgs, string, bool> showTooltip,
        Func<Task> refresh )
    {
        Type = type;
        Options = options;
        PlotArea = plotArea;
        Labels = labels;
        Series = series;
        IsRadial = radial;
        ValueMin = valueMin;
        ValueMax = valueMax;
        CategorySlotCount = categorySlotCount;
        CategoryScaleKind = categoryScaleKind;
        ContinuousCategoryAxis = categoryScaleKind == SvgChartAxisScaleKind.Continuous;
        EventReceiver = eventReceiver;
        this.projectCategory = projectCategory;
        this.projectCategoryBoundary = projectCategoryBoundary;
        this.projectX = projectX;
        this.projectValueX = projectValueX;
        this.projectY = projectY;
        this.projectAnnotationX = projectAnnotationX;
        this.projectAnnotationY = projectAnnotationY;
        this.isDataPointHidden = isDataPointHidden;
        this.pointClicked = pointClicked;
        this.pointHovered = pointHovered;
        this.dataLabelClicked = dataLabelClicked;
        this.dataLabelHovered = dataLabelHovered;
        this.pointLeft = pointLeft;
        this.showTooltip = showTooltip;
        this.refresh = refresh;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Projects a category index to the center X coordinate of the category slot.
    /// </summary>
    /// <param name="index">The category index.</param>
    /// <param name="categoryAxisId">The optional category axis identifier.</param>
    /// <returns>The projected X coordinate.</returns>
    public double ProjectCategory( double index, string categoryAxisId = null )
    {
        return projectCategory( index, categoryAxisId );
    }

    /// <summary>
    /// Projects a category boundary index to an X coordinate.
    /// </summary>
    /// <param name="index">The category boundary index.</param>
    /// <param name="categoryAxisId">The optional category axis identifier.</param>
    /// <returns>The projected X coordinate.</returns>
    public double ProjectCategoryBoundary( double index, string categoryAxisId = null )
    {
        return projectCategoryBoundary( index, categoryAxisId );
    }

    /// <summary>
    /// Projects a chart-domain X value to an X coordinate for annotations.
    /// </summary>
    /// <param name="value">The optional annotation X value.</param>
    /// <param name="fallback">The fallback coordinate to use when the value is null.</param>
    /// <returns>The projected X coordinate.</returns>
    public double ProjectAnnotationX( double? value, double fallback )
    {
        return projectAnnotationX( value, fallback );
    }

    /// <summary>
    /// Projects a chart-domain Y value to a Y coordinate for annotations.
    /// </summary>
    /// <param name="value">The optional annotation Y value.</param>
    /// <param name="valueAxisId">The optional value axis identifier.</param>
    /// <param name="fallback">The fallback coordinate to use when the value is null.</param>
    /// <returns>The projected Y coordinate.</returns>
    public double ProjectAnnotationY( double? value, string valueAxisId, double fallback )
    {
        return projectAnnotationY( value, valueAxisId, fallback );
    }

    /// <summary>
    /// Projects a numeric X value to an X coordinate.
    /// </summary>
    /// <param name="value">The numeric X value.</param>
    /// <param name="valueAxisId">The optional value axis identifier for horizontal value-axis charts.</param>
    /// <returns>The projected X coordinate.</returns>
    public double ProjectX( double value, string valueAxisId = null )
    {
        return projectX( value, valueAxisId );
    }

    /// <summary>
    /// Projects a numeric value to an X coordinate using a horizontal value axis.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="valueAxisId">The optional value axis identifier.</param>
    /// <returns>The projected X coordinate.</returns>
    public double ProjectValueX( double value, string valueAxisId = null )
    {
        return projectValueX( value, valueAxisId );
    }

    /// <summary>
    /// Projects a numeric Y value to a Y coordinate.
    /// </summary>
    /// <param name="value">The numeric Y value.</param>
    /// <param name="valueAxisId">The optional value axis identifier.</param>
    /// <returns>The projected Y coordinate.</returns>
    public double ProjectY( double value, string valueAxisId = null )
    {
        return projectY( value, valueAxisId );
    }

    /// <summary>
    /// Gets whether a radial data point is hidden.
    /// </summary>
    /// <param name="seriesName">The series name.</param>
    /// <param name="pointIndex">The point index.</param>
    /// <returns>True when the data point is hidden.</returns>
    public bool IsDataPointHidden( string seriesName, int pointIndex )
    {
        return isDataPointHidden( seriesName, pointIndex );
    }

    /// <summary>
    /// Creates a chart point event argument.
    /// </summary>
    /// <param name="series">The resolved plugin series.</param>
    /// <param name="pointIndex">The point index.</param>
    /// <param name="value">The point value.</param>
    /// <param name="bounds">The rendered point bounds.</param>
    /// <returns>The point event arguments.</returns>
    public SvgChartPointEventArgs CreatePoint( SvgChartPluginSeries series, int pointIndex, double value, SvgChartPointBounds bounds )
    {
        return CreatePoint( series, pointIndex, pointIndex >= 0 && pointIndex < Labels.Count ? Labels[pointIndex] : null, value, bounds );
    }

    /// <summary>
    /// Creates a chart point event argument.
    /// </summary>
    /// <param name="series">The resolved plugin series.</param>
    /// <param name="pointIndex">The point index.</param>
    /// <param name="category">The point category.</param>
    /// <param name="value">The point value.</param>
    /// <param name="bounds">The rendered point bounds.</param>
    /// <returns>The point event arguments.</returns>
    public SvgChartPointEventArgs CreatePoint( SvgChartPluginSeries series, int pointIndex, object category, double value, SvgChartPointBounds bounds )
    {
        return new()
        {
            SeriesName = series.Name,
            SeriesIndex = ResolveSeriesIndex( series ),
            PointIndex = pointIndex,
            Category = category,
            Value = value,
            Bounds = bounds
        };
    }

    /// <summary>
    /// Dispatches a point click through the host chart.
    /// </summary>
    /// <param name="point">The point event arguments.</param>
    /// <param name="color">The rendered point color.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task NotifyPointClicked( SvgChartPointEventArgs point, string color )
    {
        return pointClicked( point, color );
    }

    /// <summary>
    /// Dispatches a point hover through the host chart.
    /// </summary>
    /// <param name="point">The point event arguments.</param>
    /// <param name="color">The rendered point color.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task NotifyPointHovered( SvgChartPointEventArgs point, string color )
    {
        return pointHovered( point, color );
    }

    /// <summary>
    /// Dispatches a data label click through the host chart.
    /// </summary>
    /// <param name="point">The point event arguments.</param>
    /// <param name="color">The rendered point color.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task NotifyDataLabelClicked( SvgChartPointEventArgs point, string color )
    {
        return dataLabelClicked( point, color );
    }

    /// <summary>
    /// Dispatches a data label hover through the host chart.
    /// </summary>
    /// <param name="point">The point event arguments.</param>
    /// <param name="color">The rendered point color.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task NotifyDataLabelHovered( SvgChartPointEventArgs point, string color )
    {
        return dataLabelHovered( point, color );
    }

    /// <summary>
    /// Clears the active point hover state through the host chart.
    /// </summary>
    public void NotifyPointLeft()
    {
        pointLeft();
    }

    /// <summary>
    /// Shows the host chart tooltip for a point.
    /// </summary>
    /// <param name="point">The point event arguments.</param>
    /// <param name="color">The rendered point color.</param>
    /// <param name="pinned">Whether the tooltip should stay pinned.</param>
    public void ShowTooltip( SvgChartPointEventArgs point, string color, bool pinned )
    {
        showTooltip( point, color, pinned );
    }

    /// <summary>
    /// Requests the chart to render again.
    /// </summary>
    /// <returns>A task that completes after the refresh request is queued.</returns>
    public Task Refresh()
    {
        return refresh();
    }

    private int ResolveSeriesIndex( SvgChartPluginSeries series )
    {
        for ( var i = 0; i < Series.Count; i++ )
        {
            if ( ReferenceEquals( Series[i], series ) )
                return i;
        }

        return -1;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the chart type.
    /// </summary>
    public SvgChartType Type { get; }

    /// <summary>
    /// Gets the resolved chart options.
    /// </summary>
    public SvgChartOptions Options { get; }

    /// <summary>
    /// Gets the chart plot area.
    /// </summary>
    public SvgChartPluginPlotArea PlotArea { get; }

    /// <summary>
    /// Gets the resolved chart labels.
    /// </summary>
    public IReadOnlyList<object> Labels { get; }

    /// <summary>
    /// Gets the resolved chart series.
    /// </summary>
    public IReadOnlyList<SvgChartPluginSeries> Series { get; }

    /// <summary>
    /// Gets whether the chart is a radial chart.
    /// </summary>
    public bool IsRadial { get; }

    /// <summary>
    /// Gets the primary value axis minimum.
    /// </summary>
    public double ValueMin { get; }

    /// <summary>
    /// Gets the primary value axis maximum.
    /// </summary>
    public double ValueMax { get; }

    /// <summary>
    /// Gets the rendered category slot count.
    /// </summary>
    public int CategorySlotCount { get; }

    /// <summary>
    /// Gets whether the category axis is projected through a continuous numeric scale.
    /// </summary>
    public bool ContinuousCategoryAxis { get; }

    internal SvgChartAxisScaleKind CategoryScaleKind { get; }

    /// <summary>
    /// Gets the event receiver that plugins should use when creating chart event callbacks.
    /// </summary>
    public object EventReceiver { get; }

    #endregion
}