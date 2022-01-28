namespace Blazorise.Video
{
    /// <summary>
    /// Defines the DRM protection type.
    /// </summary>
    public enum VideoProtectionType
    {
        /// <summary>
        /// Video doesn't need protection data.
        /// </summary>
        None,

        /// <summary>
        /// Video is protected with PlayReady.
        /// </summary>
        PlayReady,

        /// <summary>
        /// Video is protected with Widevine.
        /// </summary>
        Widevine,

        /// <summary>
        /// Video is protected with FairPlay.
        /// </summary>
        FairPlay,
    }
}
