namespace Blazorise.Video;

internal static class Extensions
{
    public static string ToStreamingLibraryString( this StreamingLibrary streamingLibrary )
    {
        return streamingLibrary switch
        {
            StreamingLibrary.Hls => "Hls",
            StreamingLibrary.Dash => "Dash",
            _ => "Html5",
        };
    }

    public static string ToVideoProtectionTypeString( this VideoProtectionType videoProtectionType )
    {
        return videoProtectionType switch
        {
            VideoProtectionType.PlayReady => "PlayReady",
            VideoProtectionType.Widevine => "Widevine",
            VideoProtectionType.FairPlay => "FairPlay",
            _ => null,
        };
    }
}