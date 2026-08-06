#region Using directives
using System.Collections.Generic;
using System.Threading;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Contains the state available to a custom element PDF renderer.
/// </summary>
public sealed class ReportElementPdfRenderContext
{
    #region Constructors

    internal ReportElementPdfRenderContext(
        ReportDefinition definition,
        ReportBandDefinition band,
        ReportCustomElementDefinition element,
        object data,
        object item,
        IReadOnlyDictionary<string, object> runningTotals,
        CancellationToken cancellationToken )
    {
        Definition = definition;
        Band = band;
        Element = element;
        Data = data;
        Item = item;
        RunningTotals = runningTotals;
        CancellationToken = cancellationToken;
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
    /// Gets the token that is cancelled when PDF generation is superseded.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    #endregion
}