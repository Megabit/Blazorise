#r "System.IO.Compression"
#r "System.IO.Compression.ZipFile"
#r "System.Net.Http"
#r "System.Text.Json"
#r "System.Xml.Linq"

using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;

await GenerateFluentUIIcons();

async Task GenerateFluentUIIcons()
{
    // Download the official Microsoft Fluent UI System Icons repository archive.
    string url = "https://codeload.github.com/microsoft/fluentui-system-icons/zip/refs/heads/main";

    using HttpClient client = new();
    using Stream stream = await client.GetStreamAsync( url );
    using MemoryStream archiveStream = new();
    await stream.CopyToAsync( archiveStream );
    archiveStream.Position = 0;
    using ZipArchive archive = new( archiveStream, ZipArchiveMode.Read );

    string archiveRoot = GetArchiveRoot( archive )
        ?? throw new InvalidOperationException( "Could not resolve GitHub archive root folder." );
    string packageVersion = ReadPackageVersion( archive, archiveRoot ) ?? "main";

    IReadOnlyDictionary<string, ZipArchiveEntry> svgEntries = archive.Entries
        .Where( x => x.FullName.StartsWith( $"{archiveRoot}/assets/", StringComparison.Ordinal ) )
        .Where( x => x.FullName.Contains( "/SVG/", StringComparison.Ordinal ) )
        .Where( x => x.Name.EndsWith( ".svg", StringComparison.Ordinal ) )
        .GroupBy( x => Path.GetFileNameWithoutExtension( x.Name ) )
        .ToDictionary( x => x.Key, x => x.First() );

    IReadOnlyList<string> iconNames = ReadFontIconNames();
    List<string> missingIcons = iconNames.Where( x => !svgEntries.ContainsKey( x ) ).ToList();

    if ( missingIcons.Count > 0 )
        throw new InvalidOperationException( $"Could not find SVG assets for: {string.Join( ", ", missingIcons )}" );

    IReadOnlyList<IconEntry> icons = iconNames
        .Select( x => ReadIcon( x, svgEntries[x] ) )
        .OrderBy( x => x.ConstantName )
        .ToList();

    WriteFluentUIIcons( icons );
    WriteFluentUIIconSprite(
        icons.Where( x => x.IconName.EndsWith( "_regular", StringComparison.Ordinal ) ),
        "wwwroot/fluentui-icons-regular.svg",
        packageVersion );
    WriteFluentUIIconSprite(
        icons.Where( x => x.IconName.EndsWith( "_filled", StringComparison.Ordinal ) ),
        "wwwroot/fluentui-icons-filled.svg",
        packageVersion );

    Console.WriteLine( $"Exported {icons.Count} Fluent UI icons." );
}

string GetArchiveRoot( ZipArchive archive )
{
    return archive.Entries
        .Select( x => x.FullName.Split( '/' ).FirstOrDefault() )
        .FirstOrDefault( x => !string.IsNullOrWhiteSpace( x ) );
}

string ReadPackageVersion( ZipArchive archive, string archiveRoot )
{
    ZipArchiveEntry entry = archive.GetEntry( $"{archiveRoot}/packages/svg-icons/package.json" );

    if ( entry is null )
        return null;

    using Stream stream = entry.Open();
    using JsonDocument document = JsonDocument.Parse( stream );

    return document.RootElement.TryGetProperty( "version", out JsonElement version )
        ? version.GetString()
        : null;
}

IReadOnlyList<string> ReadFontIconNames()
{
    string css = File.ReadAllText( "wwwroot/FluentSystemIcons-Resizable.css" );
    MatchCollection matches = Regex.Matches( css, @"\.icon-(ic_fluent_[a-z0-9_]+_20_(?:regular|filled)):before" );

    // Preserve constants that were previously published even though their selectors are absent from the embedded CSS.
    string[] legacyIconNames =
    {
        "ic_fluent_call_chevron_down_20_regular",
        "ic_fluent_call_chevron_down_20_filled",
        "ic_fluent_device_meeting_room_microphone_above_20_regular",
        "ic_fluent_device_meeting_room_microphone_above_20_filled",
        "ic_fluent_device_meeting_room_microphone_below_20_regular",
        "ic_fluent_device_meeting_room_microphone_below_20_filled",
        "ic_fluent_error_circle_hint_20_regular",
        "ic_fluent_error_circle_hint_20_filled",
        "ic_fluent_headphones_convertible_20_regular",
        "ic_fluent_headphones_convertible_20_filled",
        "ic_fluent_headphones_monoaural_20_regular",
        "ic_fluent_headphones_monoaural_20_filled",
        "ic_fluent_lightbulb_pulse_20_regular",
        "ic_fluent_lightbulb_pulse_20_filled",
        "ic_fluent_microphone_chat_20_regular",
        "ic_fluent_microphone_chat_20_filled",
        "ic_fluent_vip_20_regular",
        "ic_fluent_vip_20_filled",
    };

    return matches.Cast<Match>()
        .Select( x => x.Groups[1].Value )
        .Concat( legacyIconNames )
        .Distinct()
        .ToList();
}

IconEntry ReadIcon( string iconName, ZipArchiveEntry entry )
{
    using Stream stream = entry.Open();
    using StreamReader reader = new( stream );
    string svg = SanitizeSvg( reader.ReadToEnd() );

    return new IconEntry( $"icon-{iconName}", GetDisplayName( iconName ), svg );
}

void WriteFluentUIIcons( IReadOnlyList<IconEntry> icons )
{
    StringBuilder builder = new();

    builder.AppendLine( "namespace Blazorise.Icons.FluentUI;" );
    builder.AppendLine();
    builder.AppendLine( "/// <summary>" );
    builder.AppendLine( "/// Generated by csx script." );
    builder.AppendLine( "/// Strongly-typed list of FluentUI icon names. (v1.1.227)" );
    builder.AppendLine( "/// </summary>" );
    builder.AppendLine( "public static class FluentUIIcons" );
    builder.AppendLine( "{" );

    foreach ( IconEntry icon in icons )
    {
        builder.AppendLine( $"    public const string {icon.ConstantName} = \"{icon.IconName}\";" );
    }

    builder.Append( "}" );

    File.WriteAllText( "FluentUIIcons.cs", NormalizeLineEndings( builder.ToString() ) );
}

void WriteFluentUIIconSprite( IEnumerable<IconEntry> icons, string fileName, string packageVersion )
{
    XNamespace svgNamespace = "http://www.w3.org/2000/svg";
    XElement sprite = new( svgNamespace + "svg" );

    foreach ( IconEntry icon in icons )
    {
        XDocument document = XDocument.Parse( icon.Svg );
        XElement symbol = new(
            svgNamespace + "symbol",
            new XAttribute( "id", icon.IconName ),
            new XAttribute( "viewBox", document.Root.Attribute( "viewBox" ).Value ),
            document.Root.Nodes() );

        sprite.Add( symbol );
    }

    string content = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>"
        + $"<!-- Generated from microsoft/fluentui-system-icons {packageVersion}. -->"
        + sprite.ToString( SaveOptions.DisableFormatting );

    File.WriteAllText( fileName, content );
}

string SanitizeSvg( string svg )
{
    XDocument document = XDocument.Parse( svg );
    XElement root = document.Root;

    root.SetAttributeValue( "class", null );
    root.SetAttributeValue( "focusable", "false" );
    root.SetAttributeValue( "aria-hidden", "true" );
    root.SetAttributeValue( "width", "1em" );
    root.SetAttributeValue( "height", "1em" );

    foreach ( XAttribute fill in root.DescendantsAndSelf().Attributes( "fill" ).Where( x => x.Value != "none" ) )
    {
        fill.Value = "currentColor";
    }

    return root.ToString( SaveOptions.DisableFormatting );
}

string GetDisplayName( string iconName )
{
    string baseName = iconName["ic_fluent_".Length..];
    bool filled = baseName.EndsWith( "_20_filled" );
    baseName = baseName
        .Replace( "_20_regular", "" )
        .Replace( "_20_filled", "" );

    string displayName = ToPascal( baseName );

    if ( filled )
        displayName += "Filled";

    return char.IsDigit( displayName.First() )
        ? $"_{displayName}"
        : displayName;
}

string ToPascal( string value )
{
    return CultureInfo.InvariantCulture.TextInfo.ToTitleCase( value.Replace( "_", " " ) ).Replace( " ", "" );
}

string NormalizeLineEndings( string value )
{
    return value
        .Replace( "\r\n", "\n" )
        .Replace( "\n", "\r\n" );
}

record IconEntry( string IconName, string ConstantName, string Svg );