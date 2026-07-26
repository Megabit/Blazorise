#region Using directives
using System.Threading.Tasks;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Designer editor for SQL report data source settings.
/// </summary>
public partial class _SqlReportDataSourceEditor
{
    #region Methods

    private Task OnConnectionNameChanged( string value )
    {
        Context?.SetValue( SqlReportDataSourceSettings.ConnectionName, value );

        return Task.CompletedTask;
    }

    private Task OnQueryChanged( string value )
    {
        Context?.SetValue( SqlReportDataSourceSettings.Query, value );

        return Task.CompletedTask;
    }

    private Task OnCommandTimeoutChanged( int? value )
    {
        Context?.SetValue( SqlReportDataSourceSettings.CommandTimeout, value );

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider settings context edited by the SQL data source editor.
    /// </summary>
    [Parameter] public ReportDataSourceProviderEditorContext Context { get; set; }

    private string ConnectionName => Context?.GetString( SqlReportDataSourceSettings.ConnectionName );

    private string Query => Context?.GetString( SqlReportDataSourceSettings.Query );

    private int? CommandTimeout => Context?.GetInteger( SqlReportDataSourceSettings.CommandTimeout );

    #endregion
}