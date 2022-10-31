using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

public class LoopConfigurationJsonConverter : JsonConverter<LoopingConfiguration>
{
    public override LoopingConfiguration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, LoopingConfiguration value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.Iterations != null ? JsonSerializer.Serialize(value.Iterations) : JsonSerializer.Serialize(value.ShouldLoop));
    }
}