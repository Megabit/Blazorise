#region Using directives
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;

public class XmlCommentToHtmlConverter
{
    readonly string[] prefixes = ["T", "P", "F", "E", "M", "N"];

    public string Convert( string xmlComment )
    {
        if ( string.IsNullOrWhiteSpace( xmlComment ) )
            return string.Empty;

        var sb = new StringBuilder();

        try
        {
            // Parse the XML comment
            var xml = XElement.Parse( $"<root>{xmlComment}</root>" );

            // Process each element and convert to HTML
            foreach ( var node in xml.Nodes() )
            {
                sb.Append( ProcessNode( node ) );
            }
        }
        catch
        {
            // Return the raw XML comment if parsing fails
            return xmlComment;
        }

        string text = sb
            .Replace( "\n", " " )
            .Replace( "\r", "" )
            .Replace( "\"", "\\\"" ) //escape quotes
            .ToString();
        return text;
    }

    private string ProcessNode( XNode node ) => node switch
    {
        XElement element => ProcessElement( element ),
        XText text => text.Value,
        _ => string.Empty
    };

    private string ProcessElement( XElement element ) => element.Name.LocalName switch
    {
        "para" => $"<p>{ProcessChildNodes( element )}</p>",
        "see" => ProcessSee( element ),
        "seealso" => ProcessSeeAlso( element ),
        "paramref" or "typeparamref" => ProcessNamedReference( element ),
        "code" => $"<pre><code>{element.Value}</code></pre>",
        "c" => $"<code>{element.Value}</code>",
        _ => ProcessChildNodes( element )// For unsupported tags, process their children
    };

    private string ProcessSee( XElement element )
    {
        string content = ProcessChildNodes( element );
        string href = element.Attribute( "href" )?.Value;

        if ( !string.IsNullOrEmpty( href ) )
            return $"<a href=\"{href}\">{( string.IsNullOrEmpty( content ) ? href : content )}</a>";

        string langword = element.Attribute( "langword" )?.Value;

        if ( !string.IsNullOrEmpty( langword ) )
            return $"<code>{( string.IsNullOrEmpty( content ) ? langword : content )}</code>";

        string cref = element.Attribute( "cref" )?.Value;

        return !string.IsNullOrEmpty( cref )
            ? $"<strong>{( string.IsNullOrEmpty( content ) ? EditCref( cref ) : content )}</strong>"
            : content;
    }

    private string ProcessSeeAlso( XElement element ) => ProcessSee( element );

    private string ProcessNamedReference( XElement element )
    {
        string content = ProcessChildNodes( element );
        string name = element.Attribute( "name" )?.Value;
        string reference = string.IsNullOrEmpty( content ) ? name : content;

        return !string.IsNullOrEmpty( reference )
            ? $"<code>{reference}</code>"
            : string.Empty;
    }

    private string ProcessChildNodes( XElement element )
    {
        var sb = new StringBuilder();
        foreach ( var child in element.Nodes() )
        {
            sb.Append( ProcessNode( child ) );
        }
        return sb.ToString();
    }

    private string EditCref( string cref )
    {
        // Remove common prefixes like "T:", "P:", "M:", "E:", etc., and "Blazorise."
        var edited = prefixes.Any( p => cref.StartsWith( $"{p}:" ) )
            ? cref.Substring( 2 )
            : cref;

        edited = Regex.Replace( edited, @"`\d+", string.Empty ); //replaces `1 (type params)
        edited = Regex.Replace( edited, "!:", string.Empty ); //replace !: (not-reference type)
        return edited.StartsWith( "Blazorise." )
               ? edited[( edited.LastIndexOf( '.' ) + 1 )..]
               : edited;
    }
}