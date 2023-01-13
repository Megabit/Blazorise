using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// JSON converter for <see cref="LoopConfiguration"/>. Used for JS interop.
/// </summary>
public class LoopConfigurationConverter : JsonConverter<LoopConfiguration>
{
    /// <inheritdoc />
    public override LoopConfiguration Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter writer, LoopConfiguration value, JsonSerializerOptions options )
    {
        writer.WriteRawValue( value.Iterations != null ? JsonSerializer.Serialize( value.Iterations ) : JsonSerializer.Serialize( value.ShouldLoop ) );
    }
}