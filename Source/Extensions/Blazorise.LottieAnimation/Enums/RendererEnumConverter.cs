using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

public class RendererEnumConverter : JsonStringEnumConverter
{
    public RendererEnumConverter() : base(new LowerCaseNamingPolicy())
    {
    }

    #region Nested type: LowerCaseNamingPolicy

    private class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }

    #endregion
}