using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// JsonConverter for a <see cref="SingleValueOrArray{TValue}"/>
/// </summary>
/// <typeparam name="TItem">Type of value or array of values</typeparam>
public class SingleValueOrArrayJsonConverter<TItem> : JsonConverter<SingleValueOrArray<TItem>>
{
    /// <summary>
    /// True if JsonConverter can be used to write values
    /// </summary>
    public bool CanWrite => true;

    /// <inheritdoc />
    public override SingleValueOrArray<TItem> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        switch ( reader.TokenType )
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartArray:
                var list = new List<TItem>();

                while ( reader.Read() )
                {
                    if ( reader.TokenType == JsonTokenType.EndArray )
                        break;

                    list.Add( JsonSerializer.Deserialize<TItem>( ref reader, options )! );
                }

                return new SingleValueOrArray<TItem>( list );
            default:
                return new List<TItem>
                {
                    JsonSerializer.Deserialize<TItem>( ref reader, options )!
                };
        }
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter writer, SingleValueOrArray<TItem> value, JsonSerializerOptions options )
    {
        if ( CanWrite && value.Values.Count == 1 )
        {
            JsonSerializer.Serialize( writer, value.Values.First(), options );
        }
        else
        {
            writer.WriteStartArray();

            foreach ( TItem item in value.Values )
                JsonSerializer.Serialize( writer, item, options );

            writer.WriteEndArray();
        }
    }
}