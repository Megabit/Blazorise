using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// JSON converter for a <see cref="Renderer"/>. Used for JSInterop.
/// </summary>
public class RendererEnumConverter : JsonStringEnumConverter
{
    /// <summary>
    /// Creates a new <see cref="RendererEnumConverter"/> instance
    /// </summary>
    public RendererEnumConverter() : base( new LowerCaseNamingPolicy() )
    {
    }

    private class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName( string name )
        {
            return name.ToLower();
        }
    }
}