using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// JSON converter for a <see cref="LottieAnimationRenderer"/>. Used for JSInterop.
/// </summary>
public class LottieAnimationRendererConverter : JsonStringEnumConverter
{
    /// <summary>
    /// Creates a new <see cref="LottieAnimationRendererConverter"/> instance.
    /// </summary>
    public LottieAnimationRendererConverter()
        : base( new LowerCaseNamingPolicy() )
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