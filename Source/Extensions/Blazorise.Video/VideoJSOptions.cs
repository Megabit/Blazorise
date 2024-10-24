using System.Text.Json.Serialization;

namespace Blazorise.Video;

internal class VideoJSOptions
{
    [JsonPropertyName( "controls" )]
    public bool Controls { get; set; }

    [JsonPropertyName( "controlsDelay" )]
    public double ControlsDelay { get; set; }

    [JsonPropertyName( "controlsList" )]
    public string[] ControlsList { get; set; }

    [JsonPropertyName( "settingsList" )]
    public VideoSettingsType[] SettingsList { get; set; }

    [JsonPropertyName( "automaticallyHideControls" )]
    public bool AutomaticallyHideControls { get; set; }

    [JsonPropertyName( "autoplay" )]
    public bool AutoPlay { get; set; }

    [JsonPropertyName( "autoPause" )]
    public bool AutoPause { get; set; }

    [JsonPropertyName( "muted" )]
    public bool Muted { get; set; }

    [JsonPropertyName( "source" )]
    public VideoSource Source { get; set; }

    [JsonPropertyName( "poster" )]
    public string Poster { get; set; }

    [JsonPropertyName( "thumbnails" )]
    public string Thumbnails { get; set; }

    [JsonPropertyName( "streamingLibrary" )]
    public string StreamingLibrary { get; set; }

    [JsonPropertyName( "seekTime" )]
    public int SeekTime { get; set; }

    [JsonPropertyName( "currentTime" )]
    public double CurrentTime { get; set; }

    [JsonPropertyName( "volume" )]
    public double Volume { get; set; }

    [JsonPropertyName( "clickToPlay" )]
    public bool ClickToPlay { get; set; }

    [JsonPropertyName( "disableContextMenu" )]
    public bool DisableContextMenu { get; set; }

    [JsonPropertyName( "resetOnEnd" )]
    public bool ResetOnEnd { get; set; }

    [JsonPropertyName( "aspectRatio" )]
    public double? AspectRatio { get; set; }

    [JsonPropertyName( "invertTime" )]
    public bool InvertTime { get; set; }

    [JsonPropertyName( "defaultQuality" )]
    public VideoJSQualityOptions DefaultQuality { get; set; }

    [JsonPropertyName( "availableQualities" )]
    public VideoJSQualityOptions[] AvailableQualities { get; set; }

    [JsonPropertyName( "protection" )]
    public VideoJSProtectionOptions Protection { get; set; }

    [JsonPropertyName( "doubleClickToFullscreen" )]
    public bool DoubleClickToFullscreen { get; set; }
}

internal class VideoJSQualityOptions
{
    public VideoJSQualityOptions()
    {
    }

    public VideoJSQualityOptions( int? height )
    {
        Height = height;
    }

    [JsonPropertyName( "height" )]
    public int? Height { get; set; }
}

internal class VideoJSProtectionOptions
{
    public VideoJSProtectionOptions()
    {
    }

    public VideoJSProtectionOptions( object data, string type, string serverUrl, string serverCertificateUrl, string httpRequestHeaders )
    {
        Data = data;
        Type = type;
        ServerUrl = serverUrl;
        ServerCertificateUrl = serverCertificateUrl;
        HttpRequestHeaders = httpRequestHeaders;
    }

    [JsonPropertyName( "data" )]
    public object Data { get; set; }

    [JsonPropertyName( "type" )]
    public string Type { get; set; }

    [JsonPropertyName( "serverUrl" )]
    public string ServerUrl { get; set; }

    [JsonPropertyName( "serverCertificateUrl" )]
    public string ServerCertificateUrl { get; set; }

    [JsonPropertyName( "httpRequestHeaders" )]
    public string HttpRequestHeaders { get; set; }
}