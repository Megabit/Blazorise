#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Serializes polymorphic annotation options using their runtime shape.
/// </summary>
public class ChartAnnotationOptionsConverter : JsonConverter<ChartAnnotationOptions>
{
    /// <inheritdoc />
    public override ChartAnnotationOptions Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter writer, ChartAnnotationOptions value, JsonSerializerOptions options )
    {
        JsonSerializer.Serialize( writer, (object)value, value.GetType() );
    }
}