using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// JsonConverter for a <see cref="JavascriptNumberOrArray"/>
/// </summary>
public class JavascriptNumberOrArrayJsonConverter : JsonConverter<JavascriptNumberOrArray>
{
    /// <inheritdoc />
    public override JavascriptNumberOrArray Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        switch ( reader.TokenType )
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartArray:
                var list = new List<JavascriptNumber>();

                while ( reader.Read() )
                {
                    if ( reader.TokenType == JsonTokenType.EndArray )
                        break;

                    list.Add( JsonSerializer.Deserialize<JavascriptNumber>( ref reader, options )! );
                }

                return new JavascriptNumberOrArray( list );
            default:
                return new List<JavascriptNumber>
                {
                    JsonSerializer.Deserialize<JavascriptNumber>( ref reader, options )!
                };
        }
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter writer, JavascriptNumberOrArray value, JsonSerializerOptions options )
    {
        if ( value.Values.Count == 1 )
            JsonSerializer.Serialize( writer, value.Values.First(), options );
        else
        {
            writer.WriteStartArray();

            foreach ( JavascriptNumber number in value.Values )
                JsonSerializer.Serialize( writer, number, options );

            writer.WriteEndArray();
        }
    }
}