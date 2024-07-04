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

        if ( parts.Length != 2 )
        {
            throw new ArgumentException( "Invalid aspect ratio format. Expected format: 'width:height'." );
        }

        if ( !int.TryParse( parts[0], out int width ) || !int.TryParse( parts[1], out int height ) )
        {
            throw new ArgumentException( "Invalid aspect ratio format. Parts should be integers." );
        }

        if ( height == 0 )
        {
            throw new ArgumentException( "Invalid aspect ratio format. Height should not be zero." );
        }

        return (double)width / height;
    }
}
