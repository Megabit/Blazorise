using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

public class AnimationSegmentJsonConverter : JsonConverter<AnimationSegment>
{
    public override AnimationSegment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, AnimationSegment value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.InitialFrame);
        writer.WriteNumberValue(value.FinalFrame);
        writer.WriteEndArray();
    }
}