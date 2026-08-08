#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        foreach ( byte value in content.Span )
        {
            if ( char.IsWhiteSpace( (char)value ) )
                continue;

            return value == (byte)'<';
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