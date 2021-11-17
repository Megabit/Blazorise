#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class IndexableOptionsConverter<T> : JsonConverter<IndexableOption<T>>
    {
        public override IndexableOption<T> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            // I only need serialization currently
            throw new NotImplementedException();
        }

        public override void Write( Utf8JsonWriter writer, IndexableOption<T> value, JsonSerializerOptions options )
        {
            if ( value.IsIndexed && value.IndexedValues != null )
                JsonSerializer.Serialize( writer, value.IndexedValues, value.IndexedValues.GetType(), options );
            else if ( value.SingleValue != null )
                JsonSerializer.Serialize( writer, value.SingleValue, value.SingleValue.GetType(), options );
        }
    }
}
