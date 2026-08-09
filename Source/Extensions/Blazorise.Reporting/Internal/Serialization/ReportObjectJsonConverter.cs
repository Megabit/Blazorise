#region Using directives
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportObjectJsonConverter : JsonConverter<object>
{
    #region Methods

    public override object Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        using JsonDocument document = JsonDocument.ParseValue( ref reader );

        return ReadElement( document.RootElement );
    }

    public override void Write( Utf8JsonWriter writer, object value, JsonSerializerOptions options )
    {
        if ( value is null )
            writer.WriteNullValue();
        else if ( value.GetType() == typeof( object ) )
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
            JsonSerializer.Serialize( writer, value, value.GetType(), options );
    }

    private static object ReadElement( JsonElement element )
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt32( out int intValue ) => intValue,
            JsonValueKind.Number when element.TryGetInt64( out long longValue ) => longValue,
            JsonValueKind.Number when element.TryGetDecimal( out decimal decimalValue ) => decimalValue,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.Array => element.EnumerateArray().Select( ReadElement ).ToList(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary( property => property.Name, property => ReadElement( property.Value ) ),
            _ => element.GetRawText(),
        };
    }

    #endregion
}