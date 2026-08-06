#region Using directives
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazorise.Reporting.Internal;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Serializes and deserializes persistent report definitions as JSON. Runtime data source objects are excluded.
/// </summary>
public static class ReportJsonSerializer
{
    #region Members

    /// <summary>
    /// Current serialized report format version.
    /// </summary>
    public const int CurrentVersion = 2;

    private static readonly JsonSerializerOptions serializerOptions = CreateOptions();

    #endregion

    #region Methods

    /// <summary>
    /// Serializes a report definition as JSON.
    /// </summary>
    /// <param name="definition">Definition to serialize.</param>
    /// <returns>Serialized report JSON.</returns>
    public static string Serialize( ReportDefinition definition )
    {
        ArgumentNullException.ThrowIfNull( definition );

        ReportDefinition serializedDefinition = ReportDefinitionHelper.EnsureDefinitionIds( ReportContext.CloneDefinition( definition ) );
        serializedDefinition.FormatVersion = CurrentVersion;

        return JsonSerializer.Serialize( serializedDefinition, serializerOptions );
    }

    /// <summary>
    /// Deserializes a report definition from JSON.
    /// </summary>
    /// <param name="json">Serialized report JSON.</param>
    /// <returns>Deserialized report definition.</returns>
    public static ReportDefinition Deserialize( string json )
    {
        if ( string.IsNullOrWhiteSpace( json ) )
            throw new ArgumentException( "Report JSON cannot be empty.", nameof( json ) );

        ReportDefinition definition = JsonSerializer.Deserialize<ReportDefinition>( json, serializerOptions )
            ?? throw new JsonException( "The report definition could not be deserialized." );

        if ( definition.FormatVersion != CurrentVersion )
            throw new NotSupportedException( $"Report format version {definition.FormatVersion} is not supported." );

        definition.FormatVersion = CurrentVersion;

        return ReportDefinitionHelper.EnsureDefinitionIds( ReportContext.CloneDefinition( definition ) );
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions( JsonSerializerDefaults.Web )
        {
            WriteIndented = true,
        };

        options.Converters.Add( new JsonStringEnumConverter() );
        options.Converters.Add( new ReportColorJsonConverter() );
        options.Converters.Add( new ReportTypeJsonConverter() );
        options.Converters.Add( new ReportObjectJsonConverter() );

        return options;
    }

    #endregion
}