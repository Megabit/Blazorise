#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Reads XML Web API responses and supports XPath element selectors.
/// </summary>
public sealed class XmlReportWebApiResponseReader : IReportWebApiResponseReader
{
    #region Members

    private static readonly byte[] Utf8Preamble = Encoding.UTF8.GetPreamble();

    private static readonly byte[] Utf16LittleEndianPreamble = Encoding.Unicode.GetPreamble();

    private static readonly byte[] Utf16BigEndianPreamble = Encoding.BigEndianUnicode.GetPreamble();

    private static readonly byte[] Utf32LittleEndianPreamble = Encoding.UTF32.GetPreamble();

    private static readonly byte[] Utf32BigEndianPreamble = new UTF32Encoding( bigEndian: true, byteOrderMark: true ).GetPreamble();

    private readonly WebApiReportDataSourceOptions options;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes an XML Web API response reader.
    /// </summary>
    /// <param name="options">Web API data source options.</param>
    public XmlReportWebApiResponseReader( WebApiReportDataSourceOptions options )
    {
        this.options = options ?? throw new ArgumentNullException( nameof( options ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public bool CanRead( string mediaType, ReadOnlyMemory<byte> content )
    {
        if ( !string.IsNullOrWhiteSpace( mediaType )
             && ( mediaType.EndsWith( "/xml", StringComparison.OrdinalIgnoreCase )
                  || mediaType.EndsWith( "+xml", StringComparison.OrdinalIgnoreCase ) ) )
            return true;

        ReadOnlySpan<byte> bytes = content.Span;

        if ( bytes.StartsWith( Utf32LittleEndianPreamble ) )
            return StartsWithUtf32XmlCharacter( bytes[Utf32LittleEndianPreamble.Length..], littleEndian: true );
        else if ( bytes.StartsWith( Utf32BigEndianPreamble ) )
            return StartsWithUtf32XmlCharacter( bytes[Utf32BigEndianPreamble.Length..], littleEndian: false );
        else if ( bytes.StartsWith( Utf8Preamble ) )
            bytes = bytes[Utf8Preamble.Length..];
        else if ( bytes.StartsWith( Utf16LittleEndianPreamble ) )
            return StartsWithUtf16XmlCharacter( bytes[Utf16LittleEndianPreamble.Length..], littleEndian: true );
        else if ( bytes.StartsWith( Utf16BigEndianPreamble ) )
            return StartsWithUtf16XmlCharacter( bytes[Utf16BigEndianPreamble.Length..], littleEndian: false );

        foreach ( byte value in bytes )
        {
            if ( char.IsWhiteSpace( (char)value ) )
                continue;

            return value == (byte)'<';
        }

        return false;
    }

    private static bool StartsWithUtf32XmlCharacter( ReadOnlySpan<byte> content, bool littleEndian )
    {
        for ( int index = 0; index + 3 < content.Length; index += 4 )
        {
            uint value = littleEndian
                ? (uint)( content[index] | content[index + 1] << 8 | content[index + 2] << 16 | content[index + 3] << 24 )
                : (uint)( content[index] << 24 | content[index + 1] << 16 | content[index + 2] << 8 | content[index + 3] );

            if ( value <= char.MaxValue && char.IsWhiteSpace( (char)value ) )
                continue;

            return value == '<';
        }

        return false;
    }

    private static bool StartsWithUtf16XmlCharacter( ReadOnlySpan<byte> content, bool littleEndian )
    {
        for ( int index = 0; index + 1 < content.Length; index += 2 )
        {
            char value = littleEndian
                ? (char)( content[index] | content[index + 1] << 8 )
                : (char)( content[index] << 8 | content[index + 1] );

            if ( char.IsWhiteSpace( value ) )
                continue;

            return value == '<';
        }

        return false;
    }

    /// <inheritdoc />
    public Task<ReportDataSourceResult> ReadAsync( ReadOnlyMemory<byte> content, string selector, CancellationToken cancellationToken = default )
    {
        cancellationToken.ThrowIfCancellationRequested();

        using MemoryStream stream = new( content.ToArray(), writable: false );
        using XmlReader reader = XmlReader.Create( stream, new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            MaxCharactersInDocument = options.MaximumResponseSize,
            XmlResolver = null,
        } );
        XDocument document = XDocument.Load( reader, LoadOptions.None );
        cancellationToken.ThrowIfCancellationRequested();

        if ( document.Root is null )
            throw new InvalidOperationException( "The Web API XML response does not contain a document element." );

        object value;

        if ( string.IsNullOrWhiteSpace( selector ) )
        {
            value = ConvertElement( document.Root, 1 );
        }
        else
        {
            List<XElement> selectedElements = document.XPathSelectElements( selector ).ToList();

            if ( selectedElements.Count > options.MaximumCollectionItems )
                throw new InvalidOperationException( $"The Web API response contains a collection larger than the configured limit of {options.MaximumCollectionItems} items." );

            value = selectedElements.Select( element => ConvertElement( element, 1 ) ).ToList();
        }

        object data = ReportWebApiDataNormalizer.NormalizeRoot( value );
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult( new ReportDataSourceResult
        {
            Data = data,
            Schema = ReportWebApiSchemaBuilder.Create( data, options.MaximumSchemaItems ),
        } );
    }

    private object ConvertElement( XElement element, int depth )
    {
        if ( depth > options.MaximumXmlDepth )
            throw new InvalidOperationException( $"The Web API XML response exceeds the configured depth limit of {options.MaximumXmlDepth}." );

        List<XElement> children = element.Elements().ToList();

        if ( children.Count == 0 && !element.HasAttributes )
            return ParseScalar( element.Value );

        Dictionary<string, object> value = new( StringComparer.OrdinalIgnoreCase );

        foreach ( XAttribute attribute in element.Attributes() )
        {
            value[$"@{attribute.Name.LocalName}"] = ParseScalar( attribute.Value );
        }

        foreach ( IGrouping<string, XElement> group in children.GroupBy( child => child.Name.LocalName, StringComparer.OrdinalIgnoreCase ) )
        {
            List<XElement> groupedChildren = group.ToList();

            if ( groupedChildren.Count > options.MaximumCollectionItems )
                throw new InvalidOperationException( $"The Web API response contains a collection larger than the configured limit of {options.MaximumCollectionItems} items." );

            value[group.Key] = groupedChildren.Count == 1
                ? ConvertElement( groupedChildren[0], depth + 1 )
                : groupedChildren.Select( child => ConvertElement( child, depth + 1 ) ).ToList();
        }

        return value;
    }

    private static object ParseScalar( string value )
    {
        if ( bool.TryParse( value, out bool boolean ) )
            return boolean;

        if ( long.TryParse( value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long integer ) )
            return integer;

        if ( decimal.TryParse( value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number ) )
            return number;

        return value;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    public string Format => WebApiReportDataSourceFormats.Xml;

    /// <inheritdoc />
    public string DisplayName => "XML";

    #endregion
}