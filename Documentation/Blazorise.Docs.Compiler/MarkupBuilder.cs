using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ColorCode;

namespace Blazorise.Docs.Compiler;

public class MarkupBuilder
{
    private readonly HtmlClassFormatter formatter;

    public MarkupBuilder( HtmlClassFormatter formatter )
    {
        this.formatter = formatter;
    }

    public string Build( string source, string language )
    {
        var cb = new CodeBuilder();

        var strippedSource = StripComponentSource( source );

        if ( language == "cs" )
        {
            cb.AddLine( "<div class=\"blazorise-codeblock\">" );

            string html = formatter.GetHtmlString( strippedSource, Languages.CSharp )
                .Replace( "@", "<span class=\"atSign\">&#64;</span>" );
            html = PreserveIndentation( html, strippedSource );

            cb.AddLine( html.ToLfLineEndings() );

            cb.AddLine( "</div>" );
        }
        else if ( language == "css" )
        {
            cb.AddLine( "<div class=\"blazorise-codeblock\">" );

            string html = formatter.GetHtmlString( strippedSource, Languages.Css )
                .Replace( "@", "<span class=\"atSign\">&#64;</span>" );
            html = PreserveIndentation( html, strippedSource );

            cb.AddLine( html.ToLfLineEndings() );

            cb.AddLine( "</div>" );
        }
        else if ( language == "powershell" )
        {
            cb.AddLine( "<div class=\"blazorise-codeblock\">" );

            string html = formatter.GetHtmlString( strippedSource, Languages.PowerShell )
                .Replace( "@", "<span class=\"atSign\">&#64;</span>" );
            html = PreserveIndentation( html, strippedSource );

            cb.AddLine( html.ToLfLineEndings() );

            cb.AddLine( "</div>" );
        }
        else
        {
            var blocks = strippedSource.Split( "@code" );

            var blocks0 = Regex.Replace( blocks[0], @"</?DocsFrame>", string.Empty )
                .Replace( "@", "PlaceholdeR" )
                .Trim();

            // Note: the @ creates problems and thus we replace it with an unlikely placeholder and in the markup replace back.
            string html = formatter.GetHtmlString( blocks0, Languages.Html ).Replace( "PlaceholdeR", "@" );
            html = AttributePostprocessing( html ).Replace( "@", "<span class=\"atSign\">&#64;</span>" );
            html = PreserveIndentation( html, blocks0 );

            cb.AddLine( "<div class=\"blazorise-codeblock\">" );
            cb.AddLine( html.ToLfLineEndings() );

            if ( blocks.Length == 2 )
            {
                string codeSource = "@code" + blocks[1];
                string codeHtml = formatter.GetHtmlString( codeSource, Languages.CSharp )
                    .Replace( "@", "<span class=\"atSign\">&#64;</span>" );
                codeHtml = PreserveIndentation( codeHtml, codeSource );

                cb.AddLine( codeHtml.ToLfLineEndings() );
            }

            cb.AddLine( "</div>" );
        }

        return cb.ToString();
    }

    private static string StripComponentSource( string source )
    {
        source = Regex.Replace( source, "@(namespace|layout|page) .+?\n", string.Empty );
        return source.Trim();
    }

    private static string PreserveIndentation( string formattedHtml, string source )
    {
        if ( string.IsNullOrEmpty( formattedHtml ) || string.IsNullOrEmpty( source ) )
            return formattedHtml;

        string[] htmlLines = formattedHtml.ToLfLineEndings().Split( '\n' );
        string[] sourceLines = source.ToLfLineEndings().Split( '\n' );

        int preStartIndex = Array.FindIndex( htmlLines, line => line.IndexOf( "<pre>", StringComparison.Ordinal ) >= 0 );
        if ( preStartIndex < 0 )
            return formattedHtml;

        int preEndIndex = Array.FindIndex( htmlLines, preStartIndex + 1, line => line.IndexOf( "</pre>", StringComparison.Ordinal ) >= 0 );
        if ( preEndIndex < 0 )
            return formattedHtml;

        int codeLineCount = preEndIndex - preStartIndex - 1;
        int lineCount = Math.Min( codeLineCount, sourceLines.Length );

        for ( int i = 0; i < lineCount; i++ )
        {
            int htmlIndex = preStartIndex + 1 + i;
            string sourceLine = sourceLines[i];
            int indentSize = GetIndentSize( sourceLine );
            string trimmedLine = htmlLines[htmlIndex].TrimStart( ' ', '\t' );

            if ( trimmedLine.Length == 0 )
            {
                htmlLines[htmlIndex] = string.Empty;
            }
            else
            {
                htmlLines[htmlIndex] = new string( ' ', indentSize ) + trimmedLine;
            }
        }

        return string.Join( "\n", htmlLines );
    }

    private static int GetIndentSize( string line )
    {
        int count = 0;

        for ( int i = 0; i < line.Length; i++ )
        {
            char current = line[i];

            if ( current == ' ' )
            {
                count++;
                continue;
            }

            if ( current == '\t' )
            {
                count += 4;
                continue;
            }

            break;
        }

        return count;
    }

    public static string AttributePostprocessing( string html )
    {
        return Regex.Replace(
            html,
            @"<span class=""htmlAttributeValue"">&quot;(?'value'.*?)&quot;</span>",
            new MatchEvaluator( m =>
            {
                var value = m.Groups["value"].Value;
                return
                    $@"<span class=""quot"">&quot;</span>{AttributeValuePostprocessing( value )}<span class=""quot"">&quot;</span>";
            } ) );
    }

    private static string AttributeValuePostprocessing( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;
        if ( value == "true" || value == "false" )
            return $"<span class=\"keyword\">{value}</span>";
        if ( Regex.IsMatch( value, "^[A-Z][A-Za-z0-9]+[.][A-Za-z][A-Za-z0-9]+$" ) )
        {
            var tokens = value.Split( '.' );
            return $"<span class=\"enum\">{tokens[0]}</span><span class=\"enumValue\">.{tokens[1]}</span>";
        }

        if ( Regex.IsMatch( value, "^@[A-Za-z0-9]+$" ) )
        {
            return $"<span class=\"sharpVariable\">{value}</span>";
        }

        return $"<span class=\"htmlAttributeValue\">{value}</span>";
    }
}