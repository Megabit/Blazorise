using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Blazorise.Docs.Compiler;

internal static class GeneratedJsonFileWriter
{
    public static void WriteIfChangedIgnoringGeneratedUtc( string outputPath, string newJson )
    {
        if ( string.IsNullOrWhiteSpace( outputPath ) )
            throw new ArgumentException( "Output path is required.", nameof( outputPath ) );

        if ( newJson is null )
            throw new ArgumentNullException( nameof( newJson ) );

        if ( !ShouldWriteFile( outputPath, newJson ) )
            return;

        File.WriteAllText( outputPath, newJson, Encoding.UTF8 );
    }

    private static bool ShouldWriteFile( string outputPath, string newJson )
    {
        if ( !File.Exists( outputPath ) )
            return true;

        string existingJson = File.ReadAllText( outputPath, Encoding.UTF8 );
        return !AreEquivalentIgnoringGeneratedUtc( existingJson, newJson );
    }

    private static bool AreEquivalentIgnoringGeneratedUtc( string existingJson, string newJson )
    {
        JsonNode existingNode = TryParseJson( existingJson );
        JsonNode newNode = TryParseJson( newJson );

        if ( existingNode is null || newNode is null )
            return string.Equals( existingJson, newJson, StringComparison.Ordinal );

        RemoveGeneratedUtc( existingNode );
        RemoveGeneratedUtc( newNode );

        return JsonNode.DeepEquals( existingNode, newNode );
    }

    private static JsonNode TryParseJson( string json )
    {
        try
        {
            return JsonNode.Parse( json );
        }
        catch ( JsonException )
        {
            return null;
        }
    }

    private static void RemoveGeneratedUtc( JsonNode node )
    {
        if ( node is JsonObject rootObject )
            rootObject.Remove( "generatedUtc" );
    }
}