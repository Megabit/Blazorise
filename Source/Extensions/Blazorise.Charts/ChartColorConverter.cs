#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

public class ChartColorConverter : JsonConverter<ChartColor>
{
    public override ChartColor Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        throw new NotImplementedException();
    }

    public override void Write( Utf8JsonWriter writer, ChartColor value, JsonSerializerOptions options )
    {
        writer.WriteStringValue( value.ToJsRgba() );
    }
}
