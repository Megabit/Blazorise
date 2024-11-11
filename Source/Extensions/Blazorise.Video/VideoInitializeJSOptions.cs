using System.Text.Json.Serialization;

namespace Blazorise.Video;

internal class VideoInitializeJSOptions
{
    public bool Controls { get; set; }

    public double ControlsDelay { get; set; }

    public string[] ControlsList { get; set; }

    public VideoSettingsType[] SettingsList { get; set; }

    public bool AutomaticallyHideControls { get; set; }

    public bool AutoPlay { get; set; }

    public bool AutoPause { get; set; }

    public bool Muted { get; set; }

    public VideoSource Source { get; set; }

    public string Poster { get; set; }

    public string Thumbnails { get; set; }

    public string StreamingLibrary { get; set; }

    public int SeekTime { get; set; }

    public double CurrentTime { get; set; }

    public double Volume { get; set; }

    public bool ClickToPlay { get; set; }

    public bool DisableContextMenu { get; set; }

    public bool ResetOnEnd { get; set; }

    public double? AspectRatio { get; set; }

    public bool InvertTime { get; set; }

    public VideoJSQualityOptions DefaultQuality { get; set; }

    public VideoJSQualityOptions[] AvailableQualities { get; set; }

    public VideoJSProtectionOptions Protection { get; set; }

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

    public object Data { get; set; }

    public string Type { get; set; }

    public string ServerUrl { get; set; }

    public string ServerCertificateUrl { get; set; }

    public string HttpRequestHeaders { get; set; }
}