#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Defines CSV report data source options.
/// </summary>
public sealed class CsvReportDataSourceOptions
{
    #region Members

    internal const long DefaultMaxSourceSize = 5 * 1024 * 1024;

    #endregion

    #region Methods

    internal void Validate()
    {
        if ( MaxSourceSize <= 0 || MaxSourceSize > int.MaxValue )
            throw new ArgumentOutOfRangeException( nameof( MaxSourceSize ), $"The maximum CSV source size must be between 1 and {int.MaxValue} bytes." );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Maximum number of bytes allowed for a CSV source.
    /// </summary>
    public long MaxSourceSize { get; set; } = DefaultMaxSourceSize;

    /// <summary>
    /// Determines whether the requested absolute CSV source URI is allowed. A null value allows every HTTP and HTTPS URI.
    /// Redirects are rejected by the built-in provider.
    /// </summary>
    public Func<Uri, bool> ResourceAllowed { get; set; }

    #endregion
}