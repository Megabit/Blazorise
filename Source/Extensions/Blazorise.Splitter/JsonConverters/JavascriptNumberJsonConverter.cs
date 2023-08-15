using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// JsonConverter for a <see cref="JavascriptNumber"/>
/// </summary>
public class JavascriptNumberJsonConverter : JsonConverter<JavascriptNumber>
{
    /// <inheritdoc />
    public override JavascriptNumber Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null or JsonTokenType.None => null,
            JsonTokenType.Number => new JavascriptNumber( reader.GetDouble() ),
            JsonTokenType.String => ParseSpecialValue( reader.GetString() ),
            _ => throw new Exception()
        };
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter writer, JavascriptNumber number, JsonSerializerOptions options )
    {
        if ( number == null )
        {
            writer.WriteNullValue();
        }
        else if ( number.IsPositiveInfinity )
        {
            writer.WriteStringValue( "Infinity" );
        }
        else if ( number.IsNegativeInfinity )
        {
            writer.WriteStringValue( "-Infinity" );
        }
        else if ( number.IsNaN )
        {
            writer.WriteStringValue( "NaN" );
        }
        else
        {
            writer.WriteNumberValue( number.Value );
        }
    }

    private JavascriptNumber ParseSpecialValue( string value )
    {
        return value switch
        {
            "Infinity" => JavascriptNumber.PositiveInfinity,
            "-Infinity" => JavascriptNumber.NegativeInfinity,
            "NaN" => JavascriptNumber.NaN,
            _ => throw new ArgumentException( "Valid values are: Infinity, -Infinity, or NaN", nameof( value ) )
        };
    }
}