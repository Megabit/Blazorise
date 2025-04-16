#r "System.Text.Json"
#r "System.Net.Http"

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

// Entry point for the script
await ExtractMaterialIcons();

async Task ExtractMaterialIcons()
{
    // Specific commit for v4.0.0 or similar
    var url = "https://raw.githubusercontent.com/google/material-design-icons/140c233b4fc1669b9b419471d94f63cc96f0106a/font/MaterialIcons-Regular.codepoints";

    // Output file path
    var outputFile = "MaterialIcons.cs";

    // Download the codepoints
    var client = new HttpClient();
    var content = await client.GetStringAsync( url );

    var lines = content.Split( '\n' );

    // Build the results from icon names
    var result = (
        from l in lines
        where !string.IsNullOrWhiteSpace( l )
        let originalName = l.Split( ' ' )[0]
        select new
        {
            DisplayName = GetDisplayName( originalName ),
            Key = originalName
        } ).OrderBy( x => x.DisplayName ).DistinctBy(x=>x.DisplayName).ToList();

    // Prepend C# class and namespace boilerplate
    string header = """
    namespace Blazorise.Icons.Material;

    /// <summary>
    /// Generated file, do not change. See README.md.
    /// Strongly-typed list of material icon names. Based on version in csx file.
    /// </summary>
    public static class MaterialIcons
    {
    """;

    // Generate the constant lines
    var constants = result
        .Select( x => $"    public const string {x.DisplayName} = \"{x.Key}\";" );

    // Write everything to file
    File.WriteAllLines( outputFile, [header, .. constants, "}"] );

    Console.WriteLine( $"Exported {result.Count} material icons to {outputFile}" );
}

// Converts icon name to PascalCase format for use as a C# identifier
string GetDisplayName( string value ) =>
    char.IsDigit( value.First() )
        ? $"_{ToPascal( value )}"
        : ToPascal( value );

// Converts snake_case to PascalCase (e.g., "arrow_back" -> "ArrowBack")
string ToPascal( string s ) =>
    CultureInfo.CurrentCulture.TextInfo.ToTitleCase( s.Replace( "_", " " ) ).Replace( " ", "" );
