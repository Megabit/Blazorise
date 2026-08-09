#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportColorJsonConverter : JsonConverter<ReportColor>
{
    #region Methods

    public override ReportColor Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        using JsonDocument document = JsonDocument.ParseValue( ref reader );
        JsonElement root = document.RootElement;
        ReportColorKind kind = (ReportColorKind)root.GetProperty( "kind" ).GetInt32();
        double alpha = root.GetProperty( "alpha" ).GetDouble();

        return kind switch
        {
            ReportColorKind.Transparent => ReportColor.Transparent,
            ReportColorKind.Named => ReportColor.FromName( root.GetProperty( "name" ).GetString(), alpha ),
            ReportColorKind.Rgb => ReportColor.FromRgb(
                root.GetProperty( "red" ).GetByte(),
                root.GetProperty( "green" ).GetByte(),
                root.GetProperty( "blue" ).GetByte(),
                alpha ),
            _ => ReportColor.Default,
        };
    }

    public override void Write( Utf8JsonWriter writer, ReportColor value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();
        writer.WriteNumber( "kind", (int)value.Kind );

        if ( value.Kind == ReportColorKind.Named )
            writer.WriteString( "name", value.Name );
        else if ( value.Kind == ReportColorKind.Rgb )
        {
            writer.WriteNumber( "red", value.Red );
            writer.WriteNumber( "green", value.Green );
            writer.WriteNumber( "blue", value.Blue );
        }

        writer.WriteNumber( "alpha", value.Alpha );
        writer.WriteEndObject();
    }

    #endregion
}