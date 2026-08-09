#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportTypeJsonConverter : JsonConverter<Type>
{
    #region Methods

    public override Type Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        string typeName = reader.GetString();

        return string.IsNullOrWhiteSpace( typeName ) ? null : Type.GetType( typeName, throwOnError: false );
    }

    public override void Write( Utf8JsonWriter writer, Type value, JsonSerializerOptions options )
    {
        writer.WriteStringValue( value?.AssemblyQualifiedName );
    }

    #endregion
}