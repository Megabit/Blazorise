#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Contains the state available to a custom element renderer.
/// </summary>
public sealed class ReportElementRenderContext
{
    #region Constructors

    internal ReportElementRenderContext(
        ReportDefinition definition,
        ReportBandDefinition band,
        ReportCustomElementDefinition element,
        object data,
        object item,
        IReadOnlyDictionary<string, object> runningTotals,
        bool designMode )
    {
        Definition = definition;
        Band = band;
        Element = element;
        Data = data;
        Item = item;
        RunningTotals = runningTotals;
        DesignMode = designMode;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the report definition being rendered.
    /// </summary>
    public ReportDefinition Definition { get; }

    /// <summary>
    /// Gets the containing band.
    /// </summary>
    public ReportBandDefinition Band { get; }

    /// <summary>
    /// Gets the custom element definition.
    /// </summary>
    public ReportCustomElementDefinition Element { get; }

    /// <summary>
    /// Gets the report data object.
    /// </summary>
    public object Data { get; }

    /// <summary>
    /// Gets the current repeated data item.
    /// </summary>
    public object Item { get; }

    /// <summary>
    /// Gets the running-total values available at this render position.
    /// </summary>
    public IReadOnlyDictionary<string, object> RunningTotals { get; }

    /// <summary>
    /// Gets a value indicating whether the element is rendered on the designer surface.
    /// </summary>
    public bool DesignMode { get; }

    #endregion
}