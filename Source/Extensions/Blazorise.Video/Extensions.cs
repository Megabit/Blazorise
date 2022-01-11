namespace Blazorise.Video
{
    internal static class Extensions
    {
        public static string ToStreamingLibrary( this StreamingLibrary streamingLibrary )
        {
            return streamingLibrary switch
            {
                StreamingLibrary.Hls => "Hls",
                StreamingLibrary.Dash => "Dash",
                _ => "Html5",
            };
        }
    }
}
