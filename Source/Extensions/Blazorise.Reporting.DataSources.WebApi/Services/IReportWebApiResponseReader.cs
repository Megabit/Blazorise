#region Using directives
using System;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Converts an HTTP response into report data and schema.
/// </summary>
public interface IReportWebApiResponseReader
{
    #region Methods

    /// <summary>
    /// Determines whether this reader can process the supplied response.
    /// </summary>
    /// <param name="mediaType">HTTP response media type, when supplied.</param>
    /// <param name="content">Buffered response content.</param>
    /// <returns><see langword="true" /> when this reader can process the response.</returns>
    bool CanRead( string mediaType, ReadOnlyMemory<byte> content );

    /// <summary>
    /// Reads report data and schema from a buffered HTTP response.
    /// </summary>
    /// <param name="content">Buffered response content.</param>
    /// <param name="selector">Optional format-specific data selector.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>The report data and inferred schema.</returns>
    Task<ReportDataSourceResult> ReadAsync( ReadOnlyMemory<byte> content, string selector, CancellationToken cancellationToken = default );

    #endregion

    #region Properties

    /// <summary>
    /// Stable response format name stored in report definitions.
    /// </summary>
    string Format { get; }

    /// <summary>
    /// User-facing response format name shown by the designer.
    /// </summary>
    string DisplayName { get; }

    #endregion
}