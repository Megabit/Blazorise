#region Using directives
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions; 
#endregion

namespace Blazorise.Docs.Compiler;

internal static class GeneratedJsonFileWriter
{
    private static readonly Regex GeneratedUtcPropertyRegex = new( "\"generatedUtc\"\\s*:\\s*\"(?:[^\"\\\\]|\\\\.)*\"", RegexOptions.Compiled );

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
        if ( AreTextuallyEquivalentIgnoringGeneratedUtc( existingJson, newJson ) )
            return true;

        existingJson = RemoveUtf8Bom( existingJson );
        newJson = RemoveUtf8Bom( newJson );

        JsonNode existingNode = TryParseJson( existingJson );
        JsonNode newNode = TryParseJson( newJson );

        if ( existingNode is null || newNode is null )
            return false;

        RemoveGeneratedUtc( existingNode );
        RemoveGeneratedUtc( newNode );

        return JsonNode.DeepEquals( existingNode, newNode );
    }

    private static bool AreTextuallyEquivalentIgnoringGeneratedUtc( string existingJson, string newJson )
    {
        string normalizedExistingJson = NormalizeJsonForTextComparison( existingJson );
        string normalizedNewJson = NormalizeJsonForTextComparison( newJson );

        return string.Equals( normalizedExistingJson, normalizedNewJson, StringComparison.Ordinal );
    }

    private static string NormalizeJsonForTextComparison( string json )
    {
        if ( json is null )
            return null;

        string normalizedJson = RemoveUtf8Bom( json );
        return GeneratedUtcPropertyRegex.Replace( normalizedJson, "\"generatedUtc\":\"__ignored__\"" );
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

    private static string RemoveUtf8Bom( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return value;

        return value[0] == '\uFEFF'
            ? value.Substring( 1 )
            : value;
    }
}