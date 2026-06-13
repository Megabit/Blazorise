#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Provides schema inference and row loading for CSV report data sources.
/// </summary>
public sealed class CsvReportDataSourceProvider : IReportDataSourceProvider
{
    #region Members

    /// <summary>
    /// Provider type used by CSV data source definitions.
    /// </summary>
    public const string ProviderType = "csv";

    private const string LegacyContentSetting = "Content";

    private const string LegacyFilePathSetting = "FilePath";

    private static readonly HttpClient httpClient = new();

    #endregion

    #region Constructors

    static CsvReportDataSourceProvider()
    {
        Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public async Task<ReportDataSourceSchema> GetSchemaAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken = default )
    {
        CsvDataSourceTable table = await ReadTableAsync( definition, cancellationToken );

        return table.Schema;
    }

    /// <inheritdoc />
    public async Task<ReportDataSourceResult> LoadDataAsync( ReportDataSourceDefinition definition, ReportDataSourceLoadContext context, CancellationToken cancellationToken = default )
    {
        CsvDataSourceTable table = await ReadTableAsync( definition, cancellationToken );

        return new()
        {
            Data = table.Rows,
            Schema = table.Schema,
        };
    }

    private static async Task<CsvDataSourceTable> ReadTableAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken )
    {
        string source = await ReadSourceAsync( definition, cancellationToken );
        char delimiter = ResolveDelimiter( definition );
        bool hasHeaderRow = ResolveBooleanSetting( definition, CsvReportDataSourceSettings.HasHeaderRow, true );
        List<List<string>> records = ParseRecords( source, delimiter );

        if ( records.Count == 0 )
            return new( [], new() { IsCollection = true } );

        List<string> headers = ResolveHeaders( records, hasHeaderRow );
        IEnumerable<List<string>> dataRecords = hasHeaderRow ? records.Skip( 1 ) : records;
        List<Type> columnTypes = InferColumnTypes( headers, dataRecords ).ToList();
        List<Dictionary<string, object>> rows = CreateRows( headers, columnTypes, dataRecords ).ToList();

        return new( rows, new()
        {
            IsCollection = true,
            Fields = headers.Select( ( header, index ) => new ReportDataSourceSchemaField
            {
                Name = header,
                DisplayName = header,
                DataType = columnTypes[index],
            } ).ToList(),
        } );
    }

    private static async Task<string> ReadSourceAsync( ReportDataSourceDefinition definition, CancellationToken cancellationToken )
    {
        string source = GetSetting( definition, CsvReportDataSourceSettings.Source )
            ?? GetSetting( definition, LegacyContentSetting )
            ?? GetSetting( definition, LegacyFilePathSetting );

        if ( string.IsNullOrWhiteSpace( source ) )
            return string.Empty;

        Encoding encoding = ResolveEncoding( definition );

        if ( IsUrlSource( source ) )
        {
            byte[] bytes = await httpClient.GetByteArrayAsync( source, cancellationToken );

            return encoding.GetString( bytes );
        }

        if ( !OperatingSystem.IsBrowser() && File.Exists( source ) )
        {
            byte[] bytes = await File.ReadAllBytesAsync( source, cancellationToken );

            return encoding.GetString( bytes );
        }

        return source;
    }

    private static Encoding ResolveEncoding( ReportDataSourceDefinition definition )
    {
        string encodingName = GetSetting( definition, CsvReportDataSourceSettings.Encoding );

        if ( string.IsNullOrWhiteSpace( encodingName ) )
            return Encoding.UTF8;

        if ( encodingName == CsvReportDataSourceSettings.SystemEncoding )
            return Encoding.Default;

        try
        {
            return Encoding.GetEncoding( encodingName );
        }
        catch ( ArgumentException )
        {
            return Encoding.UTF8;
        }
        catch ( NotSupportedException )
        {
            return Encoding.UTF8;
        }
    }

    private static bool IsUrlSource( string source )
    {
        return Uri.TryCreate( source, UriKind.Absolute, out Uri uri )
            && ( uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps );
    }

    private static char ResolveDelimiter( ReportDataSourceDefinition definition )
    {
        string delimiter = GetSetting( definition, CsvReportDataSourceSettings.Delimiter );

        return string.IsNullOrEmpty( delimiter ) ? ',' : delimiter[0];
    }

    private static bool ResolveBooleanSetting( ReportDataSourceDefinition definition, string key, bool defaultValue )
    {
        if ( definition?.Settings is null || !definition.Settings.TryGetValue( key, out object value ) )
            return defaultValue;

        return value switch
        {
            bool boolValue => boolValue,
            string stringValue when bool.TryParse( stringValue, out bool parsedValue ) => parsedValue,
            _ => defaultValue,
        };
    }

    private static string GetSetting( ReportDataSourceDefinition definition, string key )
    {
        if ( definition?.Settings is null || !definition.Settings.TryGetValue( key, out object value ) )
            return null;

        return Convert.ToString( value, CultureInfo.InvariantCulture );
    }

    private static List<string> ResolveHeaders( List<List<string>> records, bool hasHeaderRow )
    {
        int columnCount = records.Max( record => record.Count );
        List<string> headers = hasHeaderRow
            ? records[0].Select( NormalizeHeader ).ToList()
            : [];

        for ( int i = headers.Count; i < columnCount; i++ )
        {
            headers.Add( $"Column{i + 1}" );
        }

        HashSet<string> usedHeaders = new( StringComparer.OrdinalIgnoreCase );

        for ( int i = 0; i < headers.Count; i++ )
        {
            string header = string.IsNullOrWhiteSpace( headers[i] ) ? $"Column{i + 1}" : headers[i];
            string uniqueHeader = header;
            int index = 2;

            while ( !usedHeaders.Add( uniqueHeader ) )
            {
                uniqueHeader = $"{header}{index}";
                index++;
            }

            headers[i] = uniqueHeader;
        }

        return headers;
    }

    private static string NormalizeHeader( string header )
    {
        if ( string.IsNullOrWhiteSpace( header ) )
            return null;

        StringBuilder builder = new();

        foreach ( char character in header.Trim() )
        {
            if ( char.IsLetterOrDigit( character ) || character == '_' )
                builder.Append( character );
            else if ( builder.Length > 0 && builder[^1] != '_' )
                builder.Append( '_' );
        }

        return builder.Length == 0 ? null : builder.ToString().Trim( '_' );
    }

    private static IEnumerable<Type> InferColumnTypes( List<string> headers, IEnumerable<List<string>> records )
    {
        List<List<string>> materializedRecords = records.ToList();

        for ( int columnIndex = 0; columnIndex < headers.Count; columnIndex++ )
        {
            List<string> values = materializedRecords
                .Select( record => columnIndex < record.Count ? record[columnIndex] : null )
                .Where( value => !string.IsNullOrWhiteSpace( value ) )
                .ToList();

            yield return InferColumnType( values );
        }
    }

    private static Type InferColumnType( IEnumerable<string> values )
    {
        List<string> materializedValues = values.ToList();

        if ( materializedValues.Count == 0 )
            return typeof( string );

        if ( materializedValues.All( value => bool.TryParse( value, out _ ) ) )
            return typeof( bool );

        if ( materializedValues.All( value => long.TryParse( value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _ ) ) )
            return typeof( long );

        if ( materializedValues.All( value => decimal.TryParse( value, NumberStyles.Number, CultureInfo.InvariantCulture, out _ ) ) )
            return typeof( decimal );

        if ( materializedValues.All( value => DateTime.TryParse( value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _ ) ) )
            return typeof( DateTime );

        return typeof( string );
    }

    private static IEnumerable<Dictionary<string, object>> CreateRows( List<string> headers, List<Type> columnTypes, IEnumerable<List<string>> records )
    {
        foreach ( List<string> record in records )
        {
            Dictionary<string, object> row = new( StringComparer.OrdinalIgnoreCase );

            for ( int columnIndex = 0; columnIndex < headers.Count; columnIndex++ )
            {
                string value = columnIndex < record.Count ? record[columnIndex] : null;

                row[headers[columnIndex]] = ConvertValue( value, columnTypes[columnIndex] );
            }

            yield return row;
        }
    }

    private static object ConvertValue( string value, Type type )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return null;

        if ( type == typeof( bool ) && bool.TryParse( value, out bool boolValue ) )
            return boolValue;

        if ( type == typeof( long ) && long.TryParse( value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue ) )
            return longValue;

        if ( type == typeof( decimal ) && decimal.TryParse( value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal decimalValue ) )
            return decimalValue;

        if ( type == typeof( DateTime ) && DateTime.TryParse( value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTimeValue ) )
            return dateTimeValue;

        return value;
    }

    private static List<List<string>> ParseRecords( string source, char delimiter )
    {
        List<List<string>> records = [];
        List<string> record = [];
        StringBuilder field = new();
        bool quoted = false;

        for ( int i = 0; i < ( source?.Length ?? 0 ); i++ )
        {
            char character = source[i];

            if ( quoted )
            {
                if ( character == '"' && i + 1 < source.Length && source[i + 1] == '"' )
                {
                    field.Append( '"' );
                    i++;
                }
                else if ( character == '"' )
                {
                    quoted = false;
                }
                else
                {
                    field.Append( character );
                }

                continue;
            }

            if ( character == '"' )
            {
                quoted = true;
            }
            else if ( character == delimiter )
            {
                record.Add( field.ToString() );
                field.Clear();
            }
            else if ( character == '\r' || character == '\n' )
            {
                if ( character == '\r' && i + 1 < source.Length && source[i + 1] == '\n' )
                    i++;

                record.Add( field.ToString() );
                field.Clear();
                records.Add( record );
                record = [];
            }
            else
            {
                field.Append( character );
            }
        }

        if ( field.Length > 0 || record.Count > 0 )
        {
            record.Add( field.ToString() );
            records.Add( record );
        }

        return records;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Type => ProviderType;

    /// <inheritdoc />
    public string DisplayName => "CSV";

    /// <inheritdoc />
    public Type EditorComponentType => typeof( _CsvReportDataSourceEditor );

    #endregion
}