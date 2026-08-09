#region Using directives
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Provides schema discovery and row loading for SQL report data sources.
/// </summary>
/// <remarks>
/// This provider executes database commands directly and is supported only in server applications.
/// </remarks>
public sealed class SqlReportDataSourceProvider : IReportDataSourceProvider
{
    #region Members

    /// <summary>
    /// Provider type used by SQL data source definitions.
    /// </summary>
    public const string ProviderType = "sql";

    private readonly IServiceProvider serviceProvider;

    private readonly SqlReportDataSourceOptions options;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new SQL report data source provider.
    /// </summary>
    /// <param name="serviceProvider">Service provider used by named connection factories.</param>
    /// <param name="options">SQL data source provider options.</param>
    public SqlReportDataSourceProvider( IServiceProvider serviceProvider, SqlReportDataSourceOptions options )
    {
        if ( OperatingSystem.IsBrowser() )
            throw new PlatformNotSupportedException( "The SQL report data source executes database commands directly and is supported only in server applications." );

        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException( nameof( serviceProvider ) );
        this.options = options ?? throw new ArgumentNullException( nameof( options ) );
        this.options.Validate();
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public async Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
    {
        string connectionName = GetRequiredSetting( definition, SqlReportDataSourceSettings.ConnectionName );
        string query = GetRequiredSetting( definition, SqlReportDataSourceSettings.Query );

        EnsureQueryAllowed( connectionName, query );

        await using DbConnection connection = CreateConnection( connectionName );
        await OpenConnection( connection, cancellationToken );
        await using DbCommand command = CreateCommand( connection, definition, query );
        await using DbDataReader reader = await command.ExecuteReaderAsync( CommandBehavior.SchemaOnly, cancellationToken );

        return CreateSchema( reader );
    }

    /// <inheritdoc />
    public async Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
    {
        string connectionName = GetRequiredSetting( definition, SqlReportDataSourceSettings.ConnectionName );
        string query = GetRequiredSetting( definition, SqlReportDataSourceSettings.Query );

        EnsureQueryAllowed( connectionName, query );

        await using DbConnection connection = CreateConnection( connectionName );
        await OpenConnection( connection, cancellationToken );
        await using DbCommand command = CreateCommand( connection, definition, query );
        await using DbDataReader reader = await command.ExecuteReaderAsync( cancellationToken );

        ReportDataSourceSchema schema = CreateSchema( reader );
        List<Dictionary<string, object>> rows = [];

        while ( await reader.ReadAsync( cancellationToken ) )
        {
            Dictionary<string, object> row = new( StringComparer.OrdinalIgnoreCase );

            for ( int i = 0; i < reader.FieldCount; i++ )
            {
                object value = await reader.IsDBNullAsync( i, cancellationToken )
                    ? null
                    : reader.GetValue( i );

                row[schema.Fields[i].Name] = value;
            }

            rows.Add( row );
        }

        return new()
        {
            Data = rows,
            Schema = schema,
        };
    }

    private void EnsureQueryAllowed( string connectionName, string query )
    {
        if ( options.QueryAllowed?.Invoke( connectionName, query ) != true )
            throw new InvalidOperationException( $"The SQL report data source query is not allowed for connection '{connectionName}'." );
    }

    private DbConnection CreateConnection( string connectionName )
    {
        if ( !options.Connections.TryGetValue( connectionName, out Func<IServiceProvider, DbConnection> factory ) )
            throw new InvalidOperationException( $"SQL report data source connection '{connectionName}' is not registered." );

        DbConnection connection = factory( serviceProvider );

        if ( connection is null )
            throw new InvalidOperationException( $"SQL report data source connection '{connectionName}' returned no connection." );

        return connection;
    }

    private DbCommand CreateCommand( DbConnection connection, ReportDataSourceDefinition definition, string query )
    {
        int commandTimeout = options.MaximumCommandTimeout;

        if ( TryGetIntegerSetting( definition, SqlReportDataSourceSettings.CommandTimeout, out int requestedCommandTimeout ) )
        {
            if ( requestedCommandTimeout <= 0 )
                throw new InvalidOperationException( $"SQL report data source setting '{SqlReportDataSourceSettings.CommandTimeout}' must be greater than zero." );

            commandTimeout = Math.Min( requestedCommandTimeout, options.MaximumCommandTimeout );
        }

        DbCommand command = connection.CreateCommand();

        command.CommandText = query;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = commandTimeout;

        return command;
    }

    private static async Task OpenConnection( DbConnection connection, CancellationToken cancellationToken )
    {
        if ( connection.State != ConnectionState.Open )
            await connection.OpenAsync( cancellationToken );
    }

    private static ReportDataSourceSchema CreateSchema( DbDataReader reader )
    {
        ReportDataSourceSchema schema = new()
        {
            IsCollection = true,
        };
        HashSet<string> usedNames = new( StringComparer.OrdinalIgnoreCase );

        for ( int i = 0; i < reader.FieldCount; i++ )
        {
            string name = CreateUniqueName( reader.GetName( i ), i, usedNames );

            schema.Fields.Add( new()
            {
                Name = name,
                DisplayName = name,
                DataType = reader.GetFieldType( i ),
            } );
        }

        return schema;
    }

    private static string CreateUniqueName( string name, int columnIndex, HashSet<string> usedNames )
    {
        string normalizedName = string.IsNullOrWhiteSpace( name ) ? $"Column{columnIndex + 1}" : name.Trim();
        string uniqueName = normalizedName;
        int index = 2;

        while ( !usedNames.Add( uniqueName ) )
        {
            uniqueName = $"{normalizedName}{index}";
            index++;
        }

        return uniqueName;
    }

    private static string GetRequiredSetting( ReportDataSourceDefinition definition, string key )
    {
        string value = GetSetting( definition, key );

        if ( string.IsNullOrWhiteSpace( value ) )
            throw new InvalidOperationException( $"SQL report data source setting '{key}' is required." );

        return value;
    }

    private static string GetSetting( ReportDataSourceDefinition definition, string key )
    {
        if ( definition?.Settings is null || !definition.Settings.TryGetValue( key, out object value ) )
            return null;

        return Convert.ToString( value, CultureInfo.InvariantCulture );
    }

    private static bool TryGetIntegerSetting( ReportDataSourceDefinition definition, string key, out int result )
    {
        result = default;

        if ( definition?.Settings is null || !definition.Settings.TryGetValue( key, out object value ) )
            return false;

        switch ( value )
        {
            case int intValue:
                result = intValue;
                return true;

            case string stringValue:
                return int.TryParse( stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result );

            default:
                return false;
        }
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Type => ProviderType;

    /// <inheritdoc />
    public string DisplayName => "SQL";

    /// <inheritdoc />
    public Type EditorComponentType => typeof( _SqlReportDataSourceEditor );

    /// <inheritdoc />
    public bool SupportsTestConnection => true;

    #endregion
}