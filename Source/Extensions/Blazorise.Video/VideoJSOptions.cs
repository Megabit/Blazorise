namespace Blazorise.Video;
/// <summary>
/// Represents JavaScript options for initializing a video player component.
/// </summary>
public class VideoJSOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether video controls are displayed.
    /// </summary>
    public bool Controls { get; set; }

    /// <summary>
    /// Gets or sets the delay in seconds before controls are shown.
    /// </summary>
    public double ControlsDelay { get; set; }

    /// <summary>
    /// Gets or sets a list of controls to be displayed or hidden.
    /// </summary>
    public string[] ControlsList { get; set; }

    /// <summary>
    /// Gets or sets a list of video settings types available in the player.
    /// </summary>
    public VideoSettingsType[] SettingsList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether controls are automatically hidden.
    /// </summary>
    public bool AutomaticallyHideControls { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video should start playing automatically.
    /// </summary>
    public bool AutoPlay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video should automatically pause when not in view.
    /// </summary>
    public bool AutoPause { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video is muted.
    /// </summary>
    public bool Muted { get; set; }

    /// <summary>
    /// Gets or sets the source configuration for the video.
    /// </summary>
    public VideoSource Source { get; set; }

    /// <summary>
    /// Gets or sets the poster image URL displayed before the video starts.
    /// </summary>
    public string Poster { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail images for video navigation.
    /// </summary>
    public string Thumbnails { get; set; }

    /// <summary>
    /// Gets or sets the streaming library to use for video playback.
    /// </summary>
    public string StreamingLibrary { get; set; }

    /// <summary>
    /// Gets or sets the time to skip when seeking forward or backward, in seconds.
    /// </summary>
    public int SeekTime { get; set; }

    /// <summary>
    /// Gets or sets the current playback time of the video in seconds.
    /// </summary>
    public double CurrentTime { get; set; }

    /// <summary>
    /// Gets or sets the volume level of the video, between 0 and 1.
    /// </summary>
    public double Volume { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether clicking on the video toggles play/pause.
    /// </summary>
    public bool ClickToPlay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the context menu is disabled.
    /// </summary>
    public bool DisableContextMenu { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video resets to the beginning when it ends.
    /// </summary>
    public bool ResetOnEnd { get; set; }

    /// <summary>
    /// Gets or sets the aspect ratio of the video.
    /// </summary>
    public double? AspectRatio { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the time is displayed in reverse countdown format.
    /// </summary>
    public bool InvertTime { get; set; }

    /// <summary>
    /// Gets or sets the default quality options for the video.
    /// </summary>
    public VideoJSQualityOptions DefaultQuality { get; set; }

    /// <summary>
    /// Gets or sets the available quality options for the video.
    /// </summary>
    public VideoJSQualityOptions[] AvailableQualities { get; set; }

    /// <summary>
    /// Gets or sets the video protection options for DRM or other access control mechanisms.
    /// </summary>
    public VideoJSProtectionOptions Protection { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether double-clicking toggles fullscreen mode.
    /// </summary>
    public bool DoubleClickToFullscreen { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a video player component dynamically.
/// </summary>
public class VideoUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the video source.
    /// </summary>
    public JSOptionChange<VideoSource> Source { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the video protection type.
    /// </summary>
    public JSOptionChange<VideoProtectionType> ProtectionType { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the video protection data.
    /// </summary>
    public JSOptionChange<object> ProtectionData { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the protection server URL.
    /// </summary>
    public JSOptionChange<string> ProtectionServerUrl { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the protection server certificate URL.
    /// </summary>
    public JSOptionChange<string> ProtectionServerCertificateUrl { get; set; }

    /// <summary>
    /// Gets or sets the option for updating HTTP request headers for protection.
    /// </summary>
    public JSOptionChange<string> ProtectionHttpRequestHeaders { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the current playback time.
    /// </summary>
    public JSOptionChange<double?> CurrentTime { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the video volume level.
    /// </summary>
    public JSOptionChange<double?> Volume { get; set; }
}

/// <summary>
/// Represents an option change in JavaScript, specifying if a change occurred and the new value.
/// </summary>
/// <typeparam name="T">The type of the value being changed.</typeparam>
public class JSOptionChange<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the option has been changed.
    /// </summary>
    public bool Changed { get; set; }

    /// <summary>
    /// Gets or sets the new value of the option.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JSOptionChange{T}"/> class with the specified change status and value.
    /// </summary>
    /// <param name="changed">A value indicating whether the option has been changed.</param>
    /// <param name="value">The new value of the option.</param>
    public JSOptionChange( bool changed, T value )
    {
        Changed = changed;
        Value = value;
    }
}

/// <summary>
/// Represents quality options for the video, including resolution height.
/// </summary>
public class VideoJSQualityOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoJSQualityOptions"/> class.
    /// </summary>
    public VideoJSQualityOptions() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoJSQualityOptions"/> class with the specified height.
    /// </summary>
    /// <param name="height">The height of the video resolution.</param>
    public VideoJSQualityOptions( int? height )
    {
        Height = height;
    }

    /// <summary>
    /// Gets or sets the height of the video resolution.
    /// </summary>
    public int? Height { get; set; }
}

/// <summary>
/// Represents video protection options, including DRM configuration and server details.
/// </summary>
public class VideoJSProtectionOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoJSProtectionOptions"/> class.
    /// </summary>
    public VideoJSProtectionOptions() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoJSProtectionOptions"/> class with specified DRM details.
    /// </summary>
    /// <param name="data">The protection data object, such as license or certificate.</param>
    /// <param name="type">The type of protection (e.g., "DRM", "AES").</param>
    /// <param name="serverUrl">The server URL for license requests.</param>
    /// <param name="serverCertificateUrl">The URL to the server certificate for secure connections.</param>
    /// <param name="httpRequestHeaders">The HTTP headers for requests to the server.</param>
    public VideoJSProtectionOptions( object data, string type, string serverUrl, string serverCertificateUrl, string httpRequestHeaders )
    {
        Data = data;
        Type = type;
        ServerUrl = serverUrl;
        ServerCertificateUrl = serverCertificateUrl;
        HttpRequestHeaders = httpRequestHeaders;
    }

    /// <summary>
    /// Gets or sets the data object used for video protection, such as license or certificate data.
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// Gets or sets the type of video protection.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the URL of the protection server.
    /// </summary>
    public string ServerUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of the server certificate for protected streams.
    /// </summary>
    public string ServerCertificateUrl { get; set; }

    /// <summary>
    /// Gets or sets the HTTP headers used for requests to the protection server.
    /// </summary>
    public string HttpRequestHeaders { get; set; }
}