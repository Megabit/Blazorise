#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

public class ChartAnnotationOptionsConverter : JsonConverter<ChartAnnotationOptions>
{
    public override ChartAnnotationOptions Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        throw new NotImplementedException();
    }

    public override void Write( Utf8JsonWriter writer, ChartAnnotationOptions value, JsonSerializerOptions options )
    {
        JsonSerializer.Serialize( writer, (object)value, value.GetType() );
    }
}
