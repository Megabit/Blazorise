using System;

namespace Blazorise.Video.Utilities;

internal static class VideoParsers
{
    public static double? ParseAspectRatio( string aspectRatio )
    {
        if ( string.IsNullOrEmpty( aspectRatio ) || !aspectRatio.Contains( ':' ) )
        {
            return null;
        }

        var parts = aspectRatio.Split( ':' );

        if ( parts.Length == 2
            && int.TryParse( parts[0], out int width )
            && int.TryParse( parts[1], out int height ) )
        {
            return (double)width / height;
        }

        return null;
    }
}
