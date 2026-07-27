#region Using directives
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting.Internal;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Provides schema and data loading support for <see cref="DataSet" /> and <see cref="DataTable" /> report data sources.
/// </summary>
public sealed class DataSetReportDataSourceProvider : IReportDataSourceProvider
{
    #region Members

    /// <summary>
    /// Provider type used by DataSet and DataTable data sources.
    /// </summary>
    public const string ProviderType = "dataset";

    /// <summary>
    /// Provider setting used to select a single table from a DataSet.
    /// </summary>
    public const string TableNameSetting = "TableName";

    #endregion

    #region Methods

    /// <inheritdoc />
    public Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
    {
        return Task.FromResult( CreateSchema( definition?.Data, ResolveTableName( definition ) ) );
    }

    /// <inheritdoc />
    public Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
    {
        object data = definition?.Data ?? context?.DefaultData;
        string tableName = ResolveTableName( definition );

        return Task.FromResult( new ReportDataSourceResult
        {
            Data = CreateData( data, tableName ),
            Schema = definition?.Schema ?? CreateSchema( data, tableName ),
        } );
    }

    private static string ResolveTableName( ReportDataSourceDefinition definition )
    {
        if ( definition?.Settings is null || !definition.Settings.TryGetValue( TableNameSetting, out object value ) )
            return null;

        return Convert.ToString( value, CultureInfo.InvariantCulture );
    }

    private static ReportDataSourceSchema CreateSchema( object data, string tableName )
    {
        return data switch
        {
            DataTable table => CreateTableSchema( table ),
            DataSet dataSet => CreateDataSetSchema( dataSet, tableName ),
            _ => new(),
        };
    }

    private static ReportDataSourceSchema CreateDataSetSchema( DataSet dataSet, string tableName )
    {
        DataTable selectedTable = ResolveTable( dataSet, tableName );

        if ( selectedTable is not null )
            return CreateTableSchema( selectedTable );

        if ( !string.IsNullOrWhiteSpace( tableName ) )
            return new()
            {
                IsCollection = true,
            };

        ReportDataSourceSchema schema = new();

        for ( int i = 0; i < dataSet.Tables.Count; i++ )
        {
            DataTable table = dataSet.Tables[i];

            schema.Fields.Add( new()
            {
                Name = ResolveTableName( table, i ),
                DisplayName = ResolveTableName( table, i ),
                DataType = typeof( DataTable ),
                IsCollection = true,
                Fields = CreateTableFields( table ),
            } );
        }

        return schema;
    }

    private static ReportDataSourceSchema CreateTableSchema( DataTable table )
    {
        return new()
        {
            IsCollection = true,
            Fields = CreateTableFields( table ),
        };
    }

    private static List<ReportDataSourceSchemaField> CreateTableFields( DataTable table )
    {
        List<ReportDataSourceSchemaField> fields = [];

        foreach ( DataColumn column in table.Columns )
        {
            fields.Add( new()
            {
                Name = column.ColumnName,
                DisplayName = string.IsNullOrWhiteSpace( column.Caption ) ? column.ColumnName : column.Caption,
                DataType = column.DataType,
            } );
        }

        return fields;
    }

    private static object CreateData( object data, string tableName )
    {
        return data switch
        {
            DataTable table => CreateRows( table ),
            DataSet dataSet => CreateDataSetData( dataSet, tableName ),
            _ => data,
        };
    }

    private static object CreateDataSetData( DataSet dataSet, string tableName )
    {
        DataTable selectedTable = ResolveTable( dataSet, tableName );

        if ( selectedTable is not null )
            return CreateRows( selectedTable );

        if ( !string.IsNullOrWhiteSpace( tableName ) )
            return new List<Dictionary<string, object>>();

        Dictionary<string, object> tables = new( StringComparer.OrdinalIgnoreCase );

        for ( int i = 0; i < dataSet.Tables.Count; i++ )
        {
            DataTable table = dataSet.Tables[i];
            tables[ResolveTableName( table, i )] = CreateRows( table );
        }

        return tables;
    }

    private static DataTable ResolveTable( DataSet dataSet, string tableName )
    {
        if ( dataSet is null )
            return null;

        if ( !string.IsNullOrWhiteSpace( tableName ) )
        {
            foreach ( DataTable table in dataSet.Tables )
            {
                if ( string.Equals( table.TableName, tableName, StringComparison.OrdinalIgnoreCase ) )
                    return table;
            }

            return null;
        }

        return dataSet.Tables.Count == 1 ? dataSet.Tables[0] : null;
    }

    private static List<Dictionary<string, object>> CreateRows( DataTable table )
    {
        List<Dictionary<string, object>> rows = [];

        foreach ( DataRow dataRow in table.Rows )
        {
            Dictionary<string, object> row = new( StringComparer.OrdinalIgnoreCase );

            foreach ( DataColumn column in table.Columns )
            {
                object value = dataRow[column];
                row[column.ColumnName] = value == DBNull.Value ? null : value;
            }

            rows.Add( row );
        }

        return rows;
    }

    private static string ResolveTableName( DataTable table, int index )
    {
        return string.IsNullOrWhiteSpace( table?.TableName )
            ? $"Table{index + 1}"
            : table.TableName;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Type => ProviderType;

    /// <inheritdoc />
    public string DisplayName => "DataSet / DataTable";

    /// <inheritdoc />
    public Type EditorComponentType => typeof( _ReportDataSetDataSourceEditor );

    #endregion
}