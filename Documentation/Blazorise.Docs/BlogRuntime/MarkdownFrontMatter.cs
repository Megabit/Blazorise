using System;

namespace Blazorise.Docs.BlogRuntime;

internal sealed class FrontMatter
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Permalink { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string ImageTitle { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public string AuthorImage { get; set; } = "";
    public string PostedOn { get; set; } = "";
    public string ReadTime { get; set; } = "";
    public string Category { get; set; } = "";
    public string MarkdownText { get; set; } = "";
}

internal static class MarkdownFrontMatter
{
    public static FrontMatter Parse( string markdownText )
    {
        var result = new FrontMatter { MarkdownText = markdownText };

        if ( markdownText.StartsWith( "---", StringComparison.Ordinal ) )
        {
            var end = markdownText.IndexOf( "---", 3, StringComparison.Ordinal );
            var fmBlock = markdownText.Substring( 3, end - 3 )
                                      .Trim()
                                      .Split( '\n', StringSplitOptions.RemoveEmptyEntries );

            foreach ( var raw in fmBlock )
            {
                var line = raw.Trim();

                // helper local
                static string Val( string l, string key )
                    => l.Substring( key.Length + 1 ).Trim();

                if ( line.StartsWith( "title:" ) )
                    result.Title = Val( line, "title:" );
                else if ( line.StartsWith( "description:" ) )
                    result.Description = Val( line, "description:" );
                else if ( line.StartsWith( "permalink:" ) )
                    result.Permalink = Val( line, "permalink:" );
                else if ( line.StartsWith( "image-url:" ) )
                    result.ImageUrl = Val( line, "image-url:" );
                else if ( line.StartsWith( "image-title:" ) )
                    result.ImageTitle = Val( line, "image-title:" );
                else if ( line.StartsWith( "author-name:" ) )
                    result.AuthorName = Val( line, "author-name:" );
                else if ( line.StartsWith( "author-image:" ) )
                    result.AuthorImage = Val( line, "author-image:" );
                else if ( line.StartsWith( "posted-on:" ) )
                    result.PostedOn = Val( line, "posted-on:" );
                else if ( line.StartsWith( "read-time:" ) )
                    result.ReadTime = Val( line, "read-time:" );
                else if ( line.StartsWith( "category:" ) )
                    result.Category = Val( line, "category:" );
            }

            result.MarkdownText = markdownText.Substring( end + 3 ).TrimStart();
        }

        // normalize once
        result.Title = Unquote( result.Title );
        result.Description = Unquote( result.Description );
        result.Permalink = Unquote( result.Permalink );
        result.ImageUrl = Unquote( result.ImageUrl );
        result.ImageTitle = Unquote( result.ImageTitle );
        result.AuthorName = Unquote( result.AuthorName );
        result.AuthorImage = Unquote( result.AuthorImage );
        result.PostedOn = Unquote( result.PostedOn );
        result.ReadTime = Unquote( result.ReadTime );
        result.Category = Unquote( result.Category );

        return result;
    }

    private static string Unquote( string? v )
    {
        var s = ( v ?? string.Empty ).Trim();
        if ( s.Length >= 2 )
        {
            var q0 = s[0];
            var q1 = s[^1];
            if ( ( q0 == '"' && q1 == '"' ) || ( q0 == '\'' && q1 == '\'' ) )
                s = s[1..^1].Trim();
        }
        return s;
    }
}
