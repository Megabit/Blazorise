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

await GenerateAntDesignIcons();

async Task GenerateAntDesignIcons()
{
    // Download the GitHub repository archive
    string url = "https://codeload.github.com/ant-design/ant-design-icons/zip/refs/heads/master";

    using HttpClient client = new();
    using Stream stream = await client.GetStreamAsync( url );
    using MemoryStream archiveStream = new();
    await stream.CopyToAsync( archiveStream );
    archiveStream.Position = 0;
    using ZipArchive archive = new( archiveStream, ZipArchiveMode.Read );

    string archiveRoot = GetArchiveRoot( archive )
        ?? throw new InvalidOperationException( "Could not resolve GitHub archive root folder." );
    string packageVersion = ReadPackageVersion( archive, archiveRoot ) ?? "6.1.1";
    string svgRoot = $"{archiveRoot}/packages/icons-svg/svg/";

    IReadOnlyList<IconEntry> icons = archive.Entries
        .Where( x => x.FullName.StartsWith( svgRoot, StringComparison.Ordinal ) )
        .Where( x => x.FullName.EndsWith( ".svg", StringComparison.Ordinal ) )
        .Select( x => ReadIcon( x ) )
        .OrderBy( x => x.ConstantName )
        .ToList();

    WriteAntDesignIcons( icons, packageVersion );
    WriteAntDesignIconSvg( icons, packageVersion );

    Console.WriteLine( $"Exported {icons.Count} Ant Design icons." );
}

string GetArchiveRoot( ZipArchive archive )
{
    return archive.Entries
        .Select( x => x.FullName.Split( '/' ).FirstOrDefault() )
        .FirstOrDefault( x => !string.IsNullOrWhiteSpace( x ) );
}

string ReadPackageVersion( ZipArchive archive, string archiveRoot )
{
    ZipArchiveEntry entry = archive.GetEntry( $"{archiveRoot}/packages/icons-react/package.json" );

    if ( entry is null )
        return null;

    using Stream stream = entry.Open();
    using JsonDocument document = JsonDocument.Parse( stream );

    return document.RootElement.TryGetProperty( "version", out JsonElement version )
        ? version.GetString()
        : null;
}

IconEntry ReadIcon( ZipArchiveEntry entry )
{
    string[] parts = entry.FullName.Split( '/' );
    string theme = parts[^2] switch
    {
        "filled" => "filled",
        "outlined" => "outlined",
        "twotone" => "twotone",
        string value => value,
    };
    string baseName = Path.GetFileNameWithoutExtension( entry.Name );
    string iconName = $"anticon-{baseName}-{theme}";

    using Stream stream = entry.Open();
    using StreamReader reader = new( stream );
    string svg = SanitizeSvg( reader.ReadToEnd(), theme );

    return new IconEntry( baseName, theme, iconName, GetDisplayName( baseName, theme ), svg );
}

void WriteAntDesignIcons( IReadOnlyList<IconEntry> icons, string packageVersion )
{
    StringBuilder builder = new();

    builder.AppendLine( "namespace Blazorise.Icons.AntDesign;" );
    builder.AppendLine();
    builder.AppendLine( "/// <summary>" );
    builder.AppendLine( "/// Generated file, do not change. See README.md." );
    builder.AppendLine( $"/// Strongly-typed list of Ant Design icon names. (@ant-design/icons v{packageVersion})" );
    builder.AppendLine( "/// </summary>" );
    builder.AppendLine( "public static class AntDesignIcons" );
    builder.AppendLine( "{" );

    foreach ( IconEntry icon in icons )
    {
        builder.AppendLine( $"    public const string {icon.ConstantName} = \"{icon.IconName}\";" );
    }

    builder.Append( "}" );

    File.WriteAllText( "AntDesignIcons.cs", builder.ToString() );
}

void WriteAntDesignIconSvg( IReadOnlyList<IconEntry> icons, string packageVersion )
{
    StringBuilder builder = new();

    builder.AppendLine( "#region Using directives" );
    builder.AppendLine( "using System.Collections.Generic;" );
    builder.AppendLine( "#endregion" );
    builder.AppendLine();
    builder.AppendLine( "namespace Blazorise.Icons.AntDesign;" );
    builder.AppendLine();
    builder.AppendLine( "/// <summary>" );
    builder.AppendLine( "/// Generated file, do not change. See README.md." );
    builder.AppendLine( $"/// Inline SVG lookup for Ant Design icons. (@ant-design/icons v{packageVersion})" );
    builder.AppendLine( "/// </summary>" );
    builder.AppendLine( "static class AntDesignIconSvg" );
    builder.AppendLine( "{" );
    builder.AppendLine( "    private static readonly Dictionary<string, string> icons = new()" );
    builder.AppendLine( "    {" );

    foreach ( IconEntry icon in icons )
    {
        builder.AppendLine( $"        {{ AntDesignIcons.{icon.ConstantName}, \"{EscapeString( icon.Svg )}\" }}," );
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
    builder.AppendLine( "    public static string ResolveIconName( string baseName, params string[] themes )" );
    builder.AppendLine( "    {" );
    builder.AppendLine( "        if ( baseName is null )" );
    builder.AppendLine( "            return null;" );
    builder.AppendLine();
    builder.AppendLine( "        if ( baseName.StartsWith( \"anticon-\" ) && icons.ContainsKey( baseName ) )" );
    builder.AppendLine( "            return baseName;" );
    builder.AppendLine();
    builder.AppendLine( "        foreach ( string theme in themes )" );
    builder.AppendLine( "        {" );
    builder.AppendLine( "            string iconName = $\"anticon-{baseName}-{theme}\";" );
    builder.AppendLine();
    builder.AppendLine( "            if ( icons.ContainsKey( iconName ) )" );
    builder.AppendLine( "                return iconName;" );
    builder.AppendLine( "        }" );
    builder.AppendLine();
    builder.AppendLine( "        return baseName.StartsWith( \"anticon-\" )" );
    builder.AppendLine( "            ? baseName" );
    builder.AppendLine( "            : $\"anticon-{baseName}-{themes[0]}\";" );
    builder.AppendLine( "    }" );
    builder.Append( "}" );

    File.WriteAllText( "AntDesignIconSvg.cs", builder.ToString() );
}

string SanitizeSvg( string svg, string theme )
{
    XDocument document = XDocument.Parse( svg );
    XElement root = document.Root;

    root.SetAttributeValue( "class", null );
    root.SetAttributeValue( "focusable", "false" );
    root.SetAttributeValue( "aria-hidden", "true" );
    root.SetAttributeValue( "width", "1em" );
    root.SetAttributeValue( "height", "1em" );
    root.SetAttributeValue( "fill", "currentColor" );

    foreach ( XElement element in root.Descendants() )
    {
        string fill = element.Attribute( "fill" )?.Value;

        if ( fill is null )
            continue;

        if ( theme == "twotone" && IsSecondaryTone( fill ) )
        {
            element.SetAttributeValue( "fill", "var(--b-ant-icon-secondary-color, #D9D9D9)" );
        }
        else
        {
            element.SetAttributeValue( "fill", "currentColor" );
        }
    }

    return root.ToString( SaveOptions.DisableFormatting );
}

bool IsSecondaryTone( string fill )
{
    return fill.Equals( "#D9D9D9", StringComparison.OrdinalIgnoreCase )
           || fill.Equals( "#E6E6E6", StringComparison.OrdinalIgnoreCase );
}

string GetDisplayName( string baseName, string theme )
{
    return $"{ToPascal( baseName )}{ToPascal( theme )}";
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

record IconEntry( string BaseName, string Theme, string IconName, string ConstantName, string Svg );