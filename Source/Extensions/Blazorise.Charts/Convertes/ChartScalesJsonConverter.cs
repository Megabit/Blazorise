#region Using directives
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Convertes;

internal class ChartScalesJsonConverter : JsonConverter<ChartScales>
{
    public override ChartScales Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        if ( reader.TokenType != JsonTokenType.StartObject )
            throw new JsonException();

        var scales = new ChartScales();
        var additionalAxes = new Dictionary<string, ChartAxis>();

        while ( reader.Read() )
        {
            if ( reader.TokenType == JsonTokenType.EndObject )
                break;

            if ( reader.TokenType != JsonTokenType.PropertyName )
                throw new JsonException();

            var propertyName = reader.GetString();

            reader.Read();

            var axis = JsonSerializer.Deserialize<ChartAxis>( ref reader, options );

            if ( propertyName == "x" )
            {
                scales.X = axis;
            }
            else if ( propertyName == "y" )
            {
                scales.Y = axis;
            }
            else if ( propertyName is not null )
            {
                additionalAxes[propertyName] = axis;
            }
        }

        if ( additionalAxes.Count > 0 )
            scales.AdditionalAxes = additionalAxes;

        return scales;
    }

    public override void Write( Utf8JsonWriter writer, ChartScales value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();

        if ( value?.X is not null )
        {
            writer.WritePropertyName( "x" );
            JsonSerializer.Serialize( writer, value.X, options );
        }

        if ( value?.Y is not null )
        {
            writer.WritePropertyName( "y" );
            JsonSerializer.Serialize( writer, value.Y, options );
        }

        if ( value?.AdditionalAxes is not null )
        {
            foreach ( var (axisName, axis) in value.AdditionalAxes )
            {
                if ( axis is null )
                    continue;

                writer.WritePropertyName( axisName );
                JsonSerializer.Serialize( writer, axis, options );
            }
        }

        writer.WriteEndObject();
    }
}
