#region Using directives
using System.Collections.Generic;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Declares a CSV data source available to report bands and fields.
/// </summary>
public partial class ReportCsvDataSource : BaseReportDataSourceComponent
{
    #region Methods

    /// <inheritdoc />
    protected override ReportDataSourceDefinition CreateDataSourceDefinition()
    {
        Dictionary<string, object> settings = [];

        if ( Source is not null )
            settings[CsvReportDataSourceSettings.Source] = Source;

        if ( Encoding is not null )
            settings[CsvReportDataSourceSettings.Encoding] = Encoding;

        settings[CsvReportDataSourceSettings.Delimiter] = Delimiter;
        settings[CsvReportDataSourceSettings.HasHeaderRow] = HasHeaderRow;

        return new()
        {
            Name = Name,
            Type = CsvReportDataSourceProvider.ProviderType,
            Settings = settings,
            Schema = Schema,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name used by report bands and fields to reference this CSV data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// CSV source stored as inline text, a local file path, or an HTTP URL.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Text encoding used when reading CSV content from a file path or URL.
    /// </summary>
    [Parameter] public string Encoding { get; set; } = "utf-8";

    /// <summary>
    /// Single-character delimiter used to split CSV fields.
    /// </summary>
    [Parameter] public string Delimiter { get; set; } = ",";

    /// <summary>
    /// Indicates whether the first CSV row contains column names.
    /// </summary>
    [Parameter] public bool HasHeaderRow { get; set; } = true;

    /// <summary>
    /// Optional field schema used instead of schema inferred from CSV content.
    /// </summary>
    [Parameter] public ReportDataSourceSchema Schema { get; set; }

    #endregion
}