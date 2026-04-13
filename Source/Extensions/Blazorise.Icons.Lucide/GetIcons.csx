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
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;

await GenerateLucideIcons();

async Task GenerateLucideIcons()
{
    // Download the GitHub repository archive.
    string url = "https://codeload.github.com/lucide-icons/lucide/zip/refs/heads/main";

    using HttpClient client = new();
    using Stream stream = await client.GetStreamAsync( url );
    using MemoryStream archiveStream = new();
    await stream.CopyToAsync( archiveStream );
    archiveStream.Position = 0;
    using ZipArchive archive = new( archiveStream, ZipArchiveMode.Read );

    string archiveRoot = GetArchiveRoot( archive )
        ?? throw new InvalidOperationException( "Could not resolve GitHub archive root folder." );
    string packageVersion = ReadPackageVersion( archive, archiveRoot ) ?? "main";
    string svgRoot = $"{archiveRoot}/icons/";

    IReadOnlyList<IconEntry> icons = archive.Entries
        .Where( x => x.FullName.StartsWith( svgRoot, StringComparison.Ordinal ) )
        .Where( x => x.FullName.EndsWith( ".svg", StringComparison.Ordinal ) )
        .Select( ReadIcon )
        .OrderBy( x => x.ConstantName )
        .ToList();

    WriteLucideIcons( icons, packageVersion );
    WriteLucideIconSvg( icons, packageVersion );

    Console.WriteLine( $"Exported {icons.Count} Lucide icons." );
}

string GetArchiveRoot( ZipArchive archive )
{
    return archive.Entries
        .Select( x => x.FullName.Split( '/' ).FirstOrDefault() )
        .FirstOrDefault( x => !string.IsNullOrWhiteSpace( x ) );
}

string ReadPackageVersion( ZipArchive archive, string archiveRoot )
{
    ZipArchiveEntry entry = archive.GetEntry( $"{archiveRoot}/packages/lucide/package.json" );

    if ( entry is null )
        return null;

    using Stream stream = entry.Open();
    using JsonDocument document = JsonDocument.Parse( stream );

    string versionValue = document.RootElement.TryGetProperty( "version", out JsonElement version )
        ? version.GetString()
        : null;

    return string.IsNullOrWhiteSpace( versionValue ) || versionValue == "0.0.1"
        ? "main"
        : versionValue;
}

IconEntry ReadIcon( ZipArchiveEntry entry )
{
    string baseName = Path.GetFileNameWithoutExtension( entry.Name );
    string iconName = $"lucide-{baseName}";

    using Stream stream = entry.Open();
    using StreamReader reader = new( stream );
    string svg = SanitizeSvg( reader.ReadToEnd() );

    return new IconEntry( baseName, iconName, GetDisplayName( baseName ), svg );
}

void WriteLucideIcons( IReadOnlyList<IconEntry> icons, string packageVersion )
{
    StringBuilder builder = new();

    builder.AppendLine( "namespace Blazorise.Icons.Lucide;" );
    builder.AppendLine();
    builder.AppendLine( "/// <summary>" );
    builder.AppendLine( "/// Generated file, do not change. See README.md." );
    builder.AppendLine( $"/// Strongly-typed list of Lucide icon names. (lucide-icons/lucide {packageVersion})" );
    builder.AppendLine( "/// </summary>" );
    builder.AppendLine( "public static class LucideIcons" );
    builder.AppendLine( "{" );

    foreach ( IconEntry icon in icons )
    {
        builder.AppendLine( $"    public const string {icon.ConstantName} = \"{icon.IconName}\";" );
    }

    builder.Append( "}" );

    File.WriteAllText( "LucideIcons.cs", builder.ToString() );
}

void WriteLucideIconSvg( IReadOnlyList<IconEntry> icons, string packageVersion )
{
    StringBuilder builder = new();

    builder.AppendLine( "#region Using directives" );
    builder.AppendLine( "using System.Collections.Generic;" );
    builder.AppendLine( "using System.Linq;" );
    builder.AppendLine( "#endregion" );
    builder.AppendLine();
    builder.AppendLine( "namespace Blazorise.Icons.Lucide;" );
    builder.AppendLine();
    builder.AppendLine( "/// <summary>" );
    builder.AppendLine( "/// Generated file, do not change. See README.md." );
    builder.AppendLine( $"/// Inline SVG lookup for Lucide icons. (lucide-icons/lucide {packageVersion})" );
    builder.AppendLine( "/// </summary>" );
    builder.AppendLine( "static class LucideIconSvg" );
    builder.AppendLine( "{" );
    builder.AppendLine( "    private static readonly Dictionary<string, string> icons = new()" );
    builder.AppendLine( "    {" );

    foreach ( IconEntry icon in icons )
    {
        builder.AppendLine( $"        {{ LucideIcons.{icon.ConstantName}, \"{EscapeString( icon.Svg )}\" }}," );
    }

    builder.AppendLine( "    };" );
    builder.AppendLine();
    builder.AppendLine( "    public static string Get( string iconName )" );
    builder.AppendLine( "    {" );
    builder.AppendLine( "        return iconName is not null && icons.TryGetValue( iconName, out string svg )" );
    builder.AppendLine( "            ? svg" );
    builder.AppendLine( "            : null;" );
    builder.AppendLine( "    }" );
    builder.AppendLine();
    builder.AppendLine( "    public static string ResolveIconName( string baseName )" );
    builder.AppendLine( "    {" );
    builder.AppendLine( "        if ( baseName is null )" );
    builder.AppendLine( "            return null;" );
    builder.AppendLine();
    builder.AppendLine( "        string iconName = NormalizeIconName( baseName );" );
    builder.AppendLine();
    builder.AppendLine( "        return iconName;" );
    builder.AppendLine( "    }" );
    builder.AppendLine();
    builder.AppendLine( "    private static string NormalizeIconName( string name )" );
    builder.AppendLine( "    {" );
    builder.AppendLine( "        if ( string.IsNullOrWhiteSpace( name ) )" );
    builder.AppendLine( "            return null;" );
    builder.AppendLine();
    builder.AppendLine( "        string[] tokens = name.Split( ' ' );" );
    builder.AppendLine( "        string iconName = tokens.FirstOrDefault( x => x.StartsWith( \"lucide-\" ) && x != \"lucide\" && !x.EndsWith( \"x\" ) );" );
    builder.AppendLine();
    builder.AppendLine( "        if ( iconName is not null )" );
    builder.AppendLine( "            return iconName;" );
    builder.AppendLine();
    builder.AppendLine( "        return name.StartsWith( \"lucide-\" )" );
    builder.AppendLine( "            ? name" );
    builder.AppendLine( "            : $\"lucide-{name}\";" );
    builder.AppendLine( "    }" );
    builder.Append( "}" );

    File.WriteAllText( "LucideIconSvg.cs", builder.ToString() );
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
    root.SetAttributeValue( "fill", "none" );
    root.SetAttributeValue( "stroke", "currentColor" );

    return root.ToString( SaveOptions.DisableFormatting );
}

string GetDisplayName( string value )
{
    string displayName = ToPascal( value );

    return char.IsDigit( displayName.First() )
        ? $"_{displayName}"
        : displayName;
}

string ToPascal( string s )
{
    return CultureInfo.InvariantCulture.TextInfo.ToTitleCase( s.Replace( "-", " " ) ).Replace( " ", "" );
}

string EscapeString( string value )
{
    return value
        .Replace( "\\", "\\\\" )
        .Replace( "\"", "\\\"" );
}

record IconEntry( string BaseName, string IconName, string ConstantName, string Svg );