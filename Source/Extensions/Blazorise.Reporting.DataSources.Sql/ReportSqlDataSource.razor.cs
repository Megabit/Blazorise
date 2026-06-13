#region Using directives
using System.Collections.Generic;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Declares a SQL data source available to report bands and fields.
/// </summary>
public partial class ReportSqlDataSource : ReportDataSourceComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override ReportDataSourceDefinition CreateDataSourceDefinition()
    {
        Dictionary<string, object> settings = [];

        if ( ConnectionName is not null )
            settings[SqlReportDataSourceSettings.ConnectionName] = ConnectionName;

        if ( Query is not null )
            settings[SqlReportDataSourceSettings.Query] = Query;

        if ( CommandTimeout is not null )
            settings[SqlReportDataSourceSettings.CommandTimeout] = CommandTimeout.Value;

        return new()
        {
            Name = Name,
            Type = SqlReportDataSourceProvider.ProviderType,
            Settings = settings,
            Schema = Schema,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name used by report bands and fields to reference this SQL data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// Name of the host-registered SQL connection factory.
    /// </summary>
    [Parameter] public string ConnectionName { get; set; }

    /// <summary>
    /// SQL query used to load report rows.
    /// </summary>
    [Parameter] public string Query { get; set; }

    /// <summary>
    /// Optional command timeout in seconds.
    /// </summary>
    [Parameter] public int? CommandTimeout { get; set; }

    /// <summary>
    /// Optional field schema used instead of schema inferred from SQL query metadata.
    /// </summary>
    [Parameter] public ReportDataSourceSchema Schema { get; set; }

    #endregion
}