#r "System.Text.Json"
#r "System.Net.Http"

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

await ExtractBootstrapIcons();

async Task ExtractBootstrapIcons()
{
    // URL pointing to the bootstrap-icons.json  (v1.11.0)
    var url = "https://raw.githubusercontent.com/twbs/icons/26a4b76ce8e643993ca0529a4c51888df1bef7bd/font/bootstrap-icons.json";

    // Output file path
    var outputFile = "BootstrapIcons.cs";

    // Download the JSON metadata
    var client = new HttpClient();
    var json = await client.GetStringAsync( url );

    // Deserialize JSON into a dictionary of icon definitions
    var values = JsonSerializer.Deserialize<Dictionary<string, int>>(
        json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true } );

    // Build results from icon names
    var result = values
        .Select( x => new
        {
            DisplayName = GetDisplayName( x.Key ),
            Value = x.Key
        } )
        .OrderBy( x => x.DisplayName )
        .ToList();

    // Prepend C# class and namespace boilerplate
    string header = """
    namespace Blazorise.Icons.Bootstrap;

    /// <summary>
    /// Generated file, do not change. See README.md.
    /// Strongly-typed list of Bootstrap icon names. (v1.11.0)
    /// </summary>
    public static class BootstrapIcons
    {
    """;

    // Generate the constant lines
    var constants = result
        .Select( x => $"    public const string {x.DisplayName} = \"bi-{x.Value}\";" );

    // Write everything to file
    File.WriteAllLines( outputFile, [header, .. constants, "}"] );

    Console.WriteLine( $"Exported {result.Count} icons to {outputFile}" );
}

// Converts icon name to PascalCase format for use as a C# identifier
string GetDisplayName( string value ) =>
    char.IsDigit( value.First() )
        ? $"_{ToPascal( value )}"    // Prefix with underscore if it starts with a digit
        : ToPascal( value );

// Converts kebab-case to PascalCase (e.g., "arrow-left" -> "ArrowLeft")
string ToPascal( string s ) =>
    CultureInfo.CurrentCulture.TextInfo.ToTitleCase( s.Replace( "-", " " ) ).Replace( " ", "" );
