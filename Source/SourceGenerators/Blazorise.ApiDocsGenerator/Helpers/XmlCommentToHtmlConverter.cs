using System.Linq;
using System.Text.RegularExpressions;

namespace Blazorise.ApiDocsGenerator.Helpers;

using System.Text;
using System.Xml.Linq;

public class XmlCommentToHtmlConverter
{
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
        "code" => $"<pre><code>{element.Value}</code></pre>",
        "c" => $"<code>{element.Value}</code>",
        _ => ProcessChildNodes( element )// For unsupported tags, process their children
    };

    private string ProcessSee( XElement element )
    {
        var href = element.Attribute( "href" )?.Value;

        if ( !string.IsNullOrEmpty( href ) )
        {
            var content = ( element.FirstNode as XText )?.Value;

            return $"<a href=\"{href}\">{content ?? href}</a>";
        }

        var cref = element.Attribute( "cref" )?.Value;

        return cref != null
            ? $"<strong>{EditCref( cref )}</strong>"
            : string.Empty;
    }

    private string ProcessSeeAlso( XElement element )
    {
        var href = element.Attribute( "href" )?.Value;
        return href != null
            ? $"<a href=\"{href}\">{href}</a>"
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

    readonly string[] prefixes = ["T", "P", "F", "E", "M", "N"];
    private string EditCref( string cref )
    {
        // Remove common prefixes like "T:", "P:", "M:", "E:", etc., and "Blazorise."
        var edited = prefixes.Any( p => cref.StartsWith( $"{p}:" ) )
            ? cref.Substring( 2 )
            : cref;

        edited = Regex.Replace( edited, @"`\d+", string.Empty );//replaces `1 (type params)
        return edited.Replace( "Blazorise.", string.Empty );
    }
}