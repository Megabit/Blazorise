using System;
using System.Linq;

namespace Blazorise.Docs.BlogRuntime;

/// <summary>
/// Resolves standalone Liquid-style YouTube placeholders used by runtime blog Markdown.
/// </summary>
internal static class YouTubeVideoPlaceholder
{
    private const string EmbedUrlPrefix = "https://www.youtube-nocookie.com/embed/";

    /// <summary>
    /// Resolves a complete Markdown paragraph containing <c>{% youtube VIDEO_ID %}</c>.
    /// Invalid or missing IDs resolve to <see langword="null" />.
    /// </summary>
    public static bool TryResolve( string blockText, out string videoId )
    {
        videoId = null;

        if ( string.IsNullOrWhiteSpace( blockText ) )
            return false;

        string placeholder = blockText.Trim();

        if ( !placeholder.StartsWith( "{%", StringComparison.Ordinal )
            || !placeholder.EndsWith( "%}", StringComparison.Ordinal ) )
            return false;

        string[] tokens = placeholder[2..^2]
            .Split( (char[])null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

        if ( tokens.Length > 0 && string.Equals( tokens[0], "youtube", StringComparison.Ordinal ) )
        {
            if ( tokens.Length == 2 )
                TryNormalizeVideoId( tokens[1], out videoId );

            return true;
        }

        return false;
    }

    public static string GetEmbedUrl( string videoId )
        => TryNormalizeVideoId( videoId, out string normalizedVideoId )
            ? $"{EmbedUrlPrefix}{normalizedVideoId}"
            : null;

    private static bool TryNormalizeVideoId( string videoId, out string normalizedVideoId )
    {
        normalizedVideoId = videoId?.Trim();

        if ( normalizedVideoId?.Length == 11
            && normalizedVideoId.All( IsYouTubeVideoIdCharacter ) )
            return true;

        normalizedVideoId = null;
        return false;
    }

    private static bool IsYouTubeVideoIdCharacter( char character )
        => character is >= 'A' and <= 'Z'
            or >= 'a' and <= 'z'
            or >= '0' and <= '9'
            or '-' or '_';
}