namespace Blazorise.Video
{
    /// <summary>
    /// Defines the streaming library used to play the video.
    /// </summary>
    public enum StreamingLibrary
    {
        /// <summary>
        /// Video will be played with the native html5 video support.
        /// </summary>
        None,

        /// <summary>
        /// Video will be played with the dash.js library. <see href="https://github.com/Dash-Industry-Forum/dash.js">see</see>.
        /// </summary>
        Dash,

        /// <summary>
        /// Video will be played with the hls.js library. <see href="https://github.com/video-dev/hls.js/">see</see>.
        /// </summary>
        Hls,
    }
}
