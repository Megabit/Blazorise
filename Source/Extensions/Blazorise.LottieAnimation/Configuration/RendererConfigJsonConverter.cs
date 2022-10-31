using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

public class RendererConfigJsonConverter : JsonConverter<BaseRendererConfig>
{
    public override BaseRendererConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, BaseRendererConfig value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(JsonSerializer.Serialize(value, value.GetType(), options));
    }
}