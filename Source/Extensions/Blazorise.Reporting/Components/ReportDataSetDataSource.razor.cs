#region Using directives
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a DataSet or DataTable data source available to report bands and fields.
/// </summary>
public partial class ReportDataSetDataSource : BaseReportDataSourceComponent
{
    #region Methods

    /// <inheritdoc />
    protected override ReportDataSourceDefinition CreateDataSourceDefinition()
    {
        return new()
        {
            Name = Name,
            ProviderType = DataSetReportDataSourceProvider.ProviderType,
            Data = DataTable ?? (object)DataSet,
            Settings = CreateSettings(),
            Schema = Schema,
        };
    }

    private Dictionary<string, object> CreateSettings()
    {
        Dictionary<string, object> settings = [];

        if ( !string.IsNullOrWhiteSpace( TableName ) )
            settings[DataSetReportDataSourceProvider.TableNameSetting] = TableName;

        return settings;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name used by report bands and fields to reference this data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// DataSet exposed to the report designer and preview renderer.
    /// </summary>
    [Parameter] public DataSet DataSet { get; set; }

    /// <summary>
    /// DataTable exposed to the report designer and preview renderer. When set, this takes precedence over <see cref="DataSet" />.
    /// </summary>
    [Parameter] public DataTable DataTable { get; set; }

    /// <summary>
    /// Optional table name used to expose a single DataSet table as this data source.
    /// </summary>
    [Parameter] public string TableName { get; set; }

    /// <summary>
    /// Optional field schema used instead of schema inferred from the DataSet or DataTable.
    /// </summary>
    [Parameter] public ReportDataSourceSchema Schema { get; set; }

    #endregion
}