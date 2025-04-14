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

// Entry point for the script
await ExtractFontAwesomeNames();

async Task ExtractFontAwesomeNames()
{
    // Specific release for wanted version
    // see the last version in "changes" inside the json
    // get the specific release for 6.x here https://github.com/FortAwesome/Font-Awesome/commits/6.x/
    var url = "https://raw.githubusercontent.com/FortAwesome/Font-Awesome/37eff7fa00de26db41183a3ad8ed0e9119fbc44b/metadata/icons.json";

    // Output file path
    var outputFile = "FontAwesomeIcons.cs";

    // Download the JSON metadata
    var client = new HttpClient();
    var json = await client.GetStringAsync(url);

    // Deserialize JSON into a dictionary of icon definitions
    var values = JsonSerializer.Deserialize<Dictionary<string, FontAwesomeIcon>>(
        json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    // Build results from icon names
    var resultsWithNames = (
        from v in values
        select new
        {
            DisplayName = GetDisplayName(v.Key),     // C#-friendly constant name
            Key = GetIconValue(v.Key, v.Value)       // e.g., "fa-camera" or "fab youtube"
        }).ToList();

    // Build results from icon aliases
    var resultsWithAliases = (
        from v in values
        where v.Value?.Aliases?.Names?.Count > 0
        from a in v.Value.Aliases.Names
        select new
        {
            DisplayName = GetDisplayName(a),         // Use alias name
            Key = GetIconValue(v.Key, v.Value)       // Always refer to the original key
        }).ToList();

    // Combine both and sort alphabetically
    var result = resultsWithNames
        .Concat(resultsWithAliases)
        .OrderBy(x => x.DisplayName)
        .ToList();

    // Prepend C# class and namespace boilerplate
    string header = """
    namespace Blazorise.Icons.FontAwesome;
    
    /// <summary>
    /// Generated file, do not change. See README.md.
    /// Strongly-typed list of font-awesome icon names. (v6.6.0)
    /// </summary>
    public static class FontAwesomeIcons
    {
    """;
    
    // Generate the constant lines
    var constants = result
        .Select(x => $"    public const string {x.DisplayName} = \"{x.Key}\";");
    
    // Write everything to file
    File.WriteAllLines(outputFile, [header, ..constants, "}"]);

    Console.WriteLine($"Exported {result.Count} icons to {outputFile}");
}

// Converts icon name (or alias) to PascalCase format for use as a C# identifier
string GetDisplayName(string value) =>
    char.IsDigit(value.First()) 
        ? $"_{ToPascal(value)}"    // Prefix with underscore if it starts with a digit
        : ToPascal(value);

// Converts kebab-case to PascalCase (e.g., "arrow-left" -> "ArrowLeft")
string ToPascal(string s) =>
    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s).Replace("-", "");

// Determines the final string value for the icon constant (fab for brands, fa- otherwise)
string GetIconValue(string iconKey, FontAwesomeIcon icon) =>
    icon?.Styles?.Contains("brands") == true 
        ? $"fab fa-{iconKey}" 
        : $"fa-{iconKey}";

// Model for the icon metadata
class FontAwesomeIcon
{
    public FontAwesomeIconAliases Aliases { get; set; }
    public List<string> Styles { get; set; }
}

// Model for the aliases section in the metadata
class FontAwesomeIconAliases
{
    public List<string> Names { get; set; }
}
