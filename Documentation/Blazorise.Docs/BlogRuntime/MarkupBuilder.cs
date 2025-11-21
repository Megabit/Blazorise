using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ColorCode;

namespace Blazorise.Docs.BlogRuntime;

public class MarkupBuilder
{
    private readonly HtmlClassFormatter formatter;

    public MarkupBuilder( HtmlClassFormatter formatter )
    {
        this.formatter = formatter;
    }

    public string Build( string source, string language )
    {
        var cb = new StringBuilder();

        var strippedSource = StripComponentSource( source );

        if ( language == "cs" )
        {
            cb.AppendLine( "<div class=\"blazorise-codeblock\">" );

            cb.AppendLine(
                formatter.GetHtmlString( strippedSource, Languages.CSharp )
                    .Replace( "@", "<span class=\"atSign\">&#64;</span>" )
                    .ToLfLineEndings() );

            cb.AppendLine( "</div>" );
        }
        else if ( language == "css" )
        {
            cb.AppendLine( "<div class=\"blazorise-codeblock\">" );

            cb.AppendLine(
                formatter.GetHtmlString( strippedSource, Languages.Css )
                    .Replace( "@", "<span class=\"atSign\">&#64;</span>" )
                    .ToLfLineEndings() );

            cb.AppendLine( "</div>" );
        }
        else if ( language == "powershell" )
        {
            cb.AppendLine( "<div class=\"blazorise-codeblock\">" );

            cb.AppendLine(
                formatter.GetHtmlString( strippedSource, Languages.PowerShell )
                    .Replace( "@", "<span class=\"atSign\">&#64;</span>" )
                    .ToLfLineEndings() );

            cb.AppendLine( "</div>" );
        }
        else
        {
            var blocks = strippedSource.Split( "@code" );

            var blocks0 = Regex.Replace( blocks[0], @"</?DocsFrame>", string.Empty )
                .Replace( "@", "PlaceholdeR" )
                .Trim();

            // Note: the @ creates problems and thus we replace it with an unlikely placeholder and in the markup replace back.
            var html = formatter.GetHtmlString( blocks0, Languages.Html ).Replace( "PlaceholdeR", "@" );
            html = AttributePostprocessing( html ).Replace( "@", "<span class=\"atSign\">&#64;</span>" );

            cb.AppendLine( "<div class=\"blazorise-codeblock\">" );
            cb.AppendLine( html.ToLfLineEndings() );

            if ( blocks.Length == 2 )
            {
                cb.AppendLine(
                    formatter.GetHtmlString( "@code" + blocks[1], Languages.CSharp )
                        .Replace( "@", "<span class=\"atSign\">&#64;</span>" )
                        .ToLfLineEndings() );
            }

            cb.AppendLine( "</div>" );
        }

        return cb.ToString();
    }

    private static string StripComponentSource( string source )
    {
        source = Regex.Replace( source, "@(namespace|layout|page) .+?\n", string.Empty );
        return source.Trim();
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