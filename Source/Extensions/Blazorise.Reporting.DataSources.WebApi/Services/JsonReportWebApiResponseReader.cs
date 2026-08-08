#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Reads JSON Web API responses and supports RFC 6901 JSON Pointer selectors.
/// </summary>
public sealed class JsonReportWebApiResponseReader : IReportWebApiResponseReader
{
    #region Members

    private readonly WebApiReportDataSourceOptions options;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a JSON Web API response reader.
    /// </summary>
    /// <param name="options">Web API data source options.</param>
    public JsonReportWebApiResponseReader( WebApiReportDataSourceOptions options )
    {
        this.options = options ?? throw new ArgumentNullException( nameof( options ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public bool CanRead( string mediaType, ReadOnlyMemory<byte> content )
    {
        if ( !string.IsNullOrWhiteSpace( mediaType )
             && ( mediaType.EndsWith( "/json", StringComparison.OrdinalIgnoreCase )
                  || mediaType.EndsWith( "+json", StringComparison.OrdinalIgnoreCase ) ) )
            return true;

        ReadOnlySpan<byte> bytes = RemoveUtf8ByteOrderMark( content ).Span;

        foreach ( byte value in bytes )
        {
            if ( char.IsWhiteSpace( (char)value ) )
                continue;

            return value is (byte)'{' or (byte)'[';
        }

        return false;
    }

    /// <inheritdoc />
    public Task<ReportDataSourceResult> ReadAsync( ReadOnlyMemory<byte> content, string selector, CancellationToken cancellationToken = default )
    {
        cancellationToken.ThrowIfCancellationRequested();

        using JsonDocument document = JsonDocument.Parse( RemoveUtf8ByteOrderMark( content ), new()
        {
            MaxDepth = options.MaximumJsonDepth,
        } );
        cancellationToken.ThrowIfCancellationRequested();

        JsonElement selectedElement = SelectElement( document.RootElement, selector );
        object data = ReportWebApiDataNormalizer.NormalizeRoot( ConvertElement( selectedElement ) );
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult( new ReportDataSourceResult
        {
            Data = data,
            Schema = ReportWebApiSchemaBuilder.Create( data, options.MaximumSchemaItems ),
        } );
    }

    private static ReadOnlyMemory<byte> RemoveUtf8ByteOrderMark( ReadOnlyMemory<byte> content )
    {
        ReadOnlySpan<byte> bytes = content.Span;

        return bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf
            ? content[3..]
            : content;
    }

    private JsonElement SelectElement( JsonElement rootElement, string selector )
    {
        if ( string.IsNullOrWhiteSpace( selector ) )
            return rootElement;

        if ( !selector.StartsWith( "/", StringComparison.Ordinal ) )
            throw new InvalidOperationException( "A JSON data selector must be an RFC 6901 JSON Pointer beginning with '/'." );

        JsonElement current = rootElement;
        string[] segments = selector[1..].Split( '/' );

        foreach ( string segment in segments )
        {
            string token = segment.Replace( "~1", "/", StringComparison.Ordinal ).Replace( "~0", "~", StringComparison.Ordinal );

            if ( current.ValueKind == JsonValueKind.Object )
            {
                if ( !current.TryGetProperty( token, out current ) )
                    throw new InvalidOperationException( "The JSON data selector did not match the response." );
            }
            else if ( current.ValueKind == JsonValueKind.Array
                      && int.TryParse( token, NumberStyles.None, CultureInfo.InvariantCulture, out int index )
                      && index >= 0
                      && index < current.GetArrayLength() )
            {
                current = current[index];
            }
            else
            {
                throw new InvalidOperationException( "The JSON data selector did not match the response." );
            }
        }

        return current;
    }

    private object ConvertElement( JsonElement element )
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => ConvertObject( element ),
            JsonValueKind.Array => ConvertArray( element ),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64( out long integer ) => integer,
            JsonValueKind.Number when element.TryGetDecimal( out decimal number ) => number,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            _ => null,
        };
    }

    private Dictionary<string, object> ConvertObject( JsonElement element )
    {
        Dictionary<string, object> value = new( StringComparer.OrdinalIgnoreCase );

        foreach ( JsonProperty property in element.EnumerateObject() )
        {
            value[property.Name] = ConvertElement( property.Value );
        }

        return value;
    }

    private List<object> ConvertArray( JsonElement element )
    {
        if ( element.GetArrayLength() > options.MaximumCollectionItems )
            throw new InvalidOperationException( $"The Web API response contains a collection larger than the configured limit of {options.MaximumCollectionItems} items." );

        List<object> values = new( element.GetArrayLength() );

        foreach ( JsonElement item in element.EnumerateArray() )
        {
            values.Add( ConvertElement( item ) );
        }

        return values;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Format => WebApiReportDataSourceFormats.Json;

    /// <inheritdoc />
    public string DisplayName => "JSON";

    #endregion
}