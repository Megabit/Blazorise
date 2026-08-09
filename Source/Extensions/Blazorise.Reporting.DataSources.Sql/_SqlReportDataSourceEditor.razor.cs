#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        IReadOnlyList<string> connectionNames = ConnectionNames;

        if ( connectionNames.Count > 0
             && !connectionNames.Contains( ConnectionName, StringComparer.OrdinalIgnoreCase ) )
        {
            Context?.SetValue( SqlReportDataSourceSettings.ConnectionName, connectionNames[0] );
        }
    }

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

    [Inject] private SqlReportDataSourceOptions Options { get; set; }

    private IReadOnlyList<string> ConnectionNames => Options?.Connections.Keys
        .OrderBy( connectionName => connectionName, StringComparer.OrdinalIgnoreCase )
        .ToList() ?? [];

    private string ConnectionName => Context?.GetString( SqlReportDataSourceSettings.ConnectionName );

    private string Query => Context?.GetString( SqlReportDataSourceSettings.Query );

    private int? CommandTimeout => Context?.GetInteger( SqlReportDataSourceSettings.CommandTimeout );

    #endregion
}