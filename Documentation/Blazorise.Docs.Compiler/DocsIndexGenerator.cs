#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
#endregion

namespace Blazorise.Docs.Compiler;

internal sealed class DocsIndexGenerator
{
    private static readonly Regex RouteRegex = new( @"^\s*@page\s+""(?<route>[^""]+)""\s*$", RegexOptions.Compiled | RegexOptions.Multiline );
    private static readonly Regex SeoRegex = new( @"<Seo\b[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline );
    private static readonly Regex SeoAttributeRegex = new( @"(?<name>\w+)\s*=\s*""(?<value>[^""]*)""", RegexOptions.Compiled | RegexOptions.IgnoreCase );
    private static readonly Regex ExampleCodeRegex = new( @"<DocsPageSectionSource\b[^>]*\bCode\s*=\s*""(?<code>[^""]+)""[^>]*>", RegexOptions.Compiled | RegexOptions.Singleline );
    private static readonly Regex ComponentApiDocsRegex = new( @"<ComponentApiDocs\b(?<attrs>(?:[^>""]|""[^""]*"")*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline );
    private static readonly Regex ComponentTypeofRegex = new( @"typeof\s*\(\s*(?<type>[^)]+)\s*\)", RegexOptions.Compiled | RegexOptions.IgnoreCase );
    private static readonly Regex DocsSectionRegex = new( @"<DocsPageSection\b[^>]*>(?<content>.*?)</DocsPageSection>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline );
    private static readonly Regex SectionHeaderRegex = new( @"<DocsPageSectionHeader\b(?<attrs>[^>]*)>(?<content>.*?)</DocsPageSectionHeader>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline );
    private static readonly Regex SectionHeaderSelfClosingRegex = new( @"<DocsPageSectionHeader\b(?<attrs>[^>]*)/>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline );
    private static readonly Regex HtmlTagRegex = new( @"<[^>]+>", RegexOptions.Compiled | RegexOptions.Singleline );
    private static readonly Regex RazorCommentRegex = new( @"@\*.*?\*@", RegexOptions.Compiled | RegexOptions.Singleline );
    private static readonly Regex WhitespaceRegex = new( @"\s+", RegexOptions.Compiled );
    private static readonly Regex ManualEntryRegex = new( @"new\(\s*""(?<url>[^""]+)""\s*,\s*""(?<name>[^""]+)""(?:\s*,\s*""(?<description>(?:[^""\\]|\\.)*)"")?\s*\)", RegexOptions.Compiled );
    private static readonly Regex RazorDirectiveRegex = new( @"@(namespace|layout|page)\s+.+?\r?\n", RegexOptions.Compiled );

    public bool Execute()
    {
        bool success = true;

        try
        {
            string docsRoot = Paths.DirPath();

            if ( string.IsNullOrWhiteSpace( docsRoot ) )
            {
                Console.WriteLine( "DocsIndexGenerator: unable to locate Blazorise.Docs directory." );
                return false;
            }

            string pagesRoot = Path.Combine( docsRoot, "Pages", "Docs" );

            if ( !Directory.Exists( pagesRoot ) )
            {
                Console.WriteLine( $"DocsIndexGenerator: pages directory not found: {pagesRoot}" );
                return false;
            }

            Dictionary<string, ManualEntry> manualEntries = LoadManualEntries( docsRoot );
            Dictionary<string, List<string>> exampleIndex = BuildExampleIndex( pagesRoot );
            List<DocsPage> pages = new List<DocsPage>();

            IEnumerable<string> pageFiles = Directory.EnumerateFiles( pagesRoot, "*.razor", SearchOption.AllDirectories );

            foreach ( string pageFile in pageFiles )
            {
                string content = File.ReadAllText( pageFile, Encoding.UTF8 );

                List<string> routes = ExtractRoutes( content );
                if ( routes.Count == 0 )
                    continue;

                List<string> docsRoutes = routes
                    .Where( route => route.StartsWith( "/docs", StringComparison.OrdinalIgnoreCase ) )
                    .ToList();

                if ( docsRoutes.Count == 0 )
                    continue;

                SeoInfo seo = ExtractSeoInfo( content );
                List<string> exampleCodes = ExtractExampleCodes( content );
                Dictionary<string, ExampleMetadata> exampleMetadata = ExtractExampleMetadata( content );
                List<DocsExample> examples = BuildExamples( pageFile, exampleCodes, exampleIndex, docsRoot, exampleMetadata );
                List<DocsApiRef> apiRefs = ExtractApiRefs( content );
                string pagePath = NormalizePath( Path.GetRelativePath( docsRoot, pageFile ) );

                foreach ( string route in docsRoutes )
                {
                    ManualEntry manualEntry = GetManualEntry( manualEntries, route );

                    string title = string.IsNullOrWhiteSpace( seo.Title )
                        ? manualEntry?.Name
                        : seo.Title;

                    string description = string.IsNullOrWhiteSpace( seo.Description )
                        ? manualEntry?.Description
                        : seo.Description;

                    DocsPage page = new DocsPage
                    {
                        Route = route,
                        Title = title,
                        Description = description,
                        PagePath = pagePath,
                        Examples = examples,
                        ApiRefs = apiRefs.Count > 0 ? apiRefs : null
                    };

                    pages.Add( page );
                }
            }

            pages.Sort( ( left, right ) => StringComparer.OrdinalIgnoreCase.Compare( left.Route, right.Route ) );

            DocsIndex index = new DocsIndex
            {
                GeneratedUtc = DateTime.UtcNow.ToString( "O", CultureInfo.InvariantCulture ),
                Pages = pages
            };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize( index, options );
            string outputPath = Paths.DocsIndexFilePath();
            string outputDirectory = Path.GetDirectoryName( outputPath );

            if ( !Directory.Exists( outputDirectory ) )
            {
                Directory.CreateDirectory( outputDirectory );
            }

            File.WriteAllText( outputPath, json, Encoding.UTF8 );
        }
        catch ( Exception e )
        {
            Console.WriteLine( $"Error generating docs index: {e.Message}" );
            success = false;
        }

        return success;
    }

    private static Dictionary<string, ManualEntry> LoadManualEntries( string docsRoot )
    {
        Dictionary<string, ManualEntry> entries = new Dictionary<string, ManualEntry>( StringComparer.OrdinalIgnoreCase );

        string manualEntriesPath = Path.Combine( docsRoot, "Services", "Search", "ManualPageEntries.cs" );

        if ( !File.Exists( manualEntriesPath ) )
            return entries;

        string content = File.ReadAllText( manualEntriesPath, Encoding.UTF8 );
        MatchCollection matches = ManualEntryRegex.Matches( content );

        foreach ( Match match in matches )
        {
            string url = UnescapeCSharpString( match.Groups["url"].Value );
            string name = UnescapeCSharpString( match.Groups["name"].Value );
            string description = match.Groups["description"].Success
                ? UnescapeCSharpString( match.Groups["description"].Value )
                : null;

            if ( string.IsNullOrWhiteSpace( url ) )
                continue;

            string route = url.StartsWith( "/", StringComparison.Ordinal ) ? url : "/" + url;

            ManualEntry entry = new ManualEntry( route, name, description );
            entries[route] = entry;
        }

        return entries;
    }

    private static Dictionary<string, List<string>> BuildExampleIndex( string pagesRoot )
    {
        Dictionary<string, List<string>> index = new Dictionary<string, List<string>>( StringComparer.OrdinalIgnoreCase );

        IEnumerable<string> exampleFiles = Directory.EnumerateFiles( pagesRoot, "*.*", SearchOption.AllDirectories )
            .Where( path => IsExampleFile( path ) );

        foreach ( string filePath in exampleFiles )
        {
            string codeName = Path.GetFileNameWithoutExtension( filePath );

            if ( !index.TryGetValue( codeName, out List<string> paths ) )
            {
                paths = new List<string>();
                index[codeName] = paths;
            }

            paths.Add( filePath );
        }

        return index;
    }

    private static bool IsExampleFile( string filePath )
    {
        string extension = Path.GetExtension( filePath );

        if ( !extension.Equals( ".razor", StringComparison.OrdinalIgnoreCase )
             && !extension.Equals( ".snippet", StringComparison.OrdinalIgnoreCase )
             && !extension.Equals( ".csharp", StringComparison.OrdinalIgnoreCase ) )
        {
            return false;
        }

        string examplesSegment = $"{Path.DirectorySeparatorChar}Examples{Path.DirectorySeparatorChar}";
        return filePath.Contains( examplesSegment, StringComparison.OrdinalIgnoreCase );
    }

    private static List<string> ExtractRoutes( string content )
    {
        List<string> routes = new List<string>();
        MatchCollection matches = RouteRegex.Matches( content );

        foreach ( Match match in matches )
        {
            string route = match.Groups["route"].Value;

            if ( !string.IsNullOrWhiteSpace( route ) )
            {
                routes.Add( route.Trim() );
            }
        }

        return routes;
    }

    private static SeoInfo ExtractSeoInfo( string content )
    {
        Match match = SeoRegex.Match( content );

        if ( !match.Success )
            return new SeoInfo( null, null );

        string tag = match.Value;
        string title = ExtractSeoAttribute( tag, "Title" );
        string description = ExtractSeoAttribute( tag, "Description" );

        if ( IsDynamicValue( title ) )
            title = null;

        if ( IsDynamicValue( description ) )
            description = null;

        return new SeoInfo( title, description );
    }

    private static string ExtractSeoAttribute( string tag, string attributeName )
    {
        MatchCollection matches = SeoAttributeRegex.Matches( tag );

        foreach ( Match match in matches )
        {
            string name = match.Groups["name"].Value;

            if ( string.Equals( name, attributeName, StringComparison.OrdinalIgnoreCase ) )
            {
                return match.Groups["value"].Value;
            }
        }

        return null;
    }

    private static bool IsDynamicValue( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return true;

        return value.TrimStart().StartsWith( "@", StringComparison.Ordinal );
    }

    private static List<string> ExtractExampleCodes( string content )
    {
        List<string> codes = new List<string>();
        HashSet<string> seen = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
        MatchCollection matches = ExampleCodeRegex.Matches( content );

        foreach ( Match match in matches )
        {
            string code = match.Groups["code"].Value;

            if ( string.IsNullOrWhiteSpace( code ) )
                continue;

            if ( seen.Add( code ) )
                codes.Add( code );
        }

        return codes;
    }

    private static Dictionary<string, ExampleMetadata> ExtractExampleMetadata( string content )
    {
        Dictionary<string, ExampleMetadata> metadata = new Dictionary<string, ExampleMetadata>( StringComparer.OrdinalIgnoreCase );
        MatchCollection sectionMatches = DocsSectionRegex.Matches( content );

        foreach ( Match sectionMatch in sectionMatches )
        {
            string sectionContent = sectionMatch.Groups["content"].Value;
            ExampleMetadata sectionMetadata = ExtractSectionMetadata( sectionContent );

            List<string> codes = ExtractExampleCodes( sectionContent );
            if ( codes.Count == 0 )
                continue;

            if ( sectionMetadata is null )
                continue;

            foreach ( string code in codes )
            {
                if ( !metadata.ContainsKey( code ) )
                {
                    metadata[code] = sectionMetadata;
                }
            }
        }

        return metadata;
    }

    private static List<DocsApiRef> ExtractApiRefs( string content )
    {
        List<DocsApiRef> apiRefs = new List<DocsApiRef>();
        HashSet<string> seen = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
        MatchCollection matches = ComponentApiDocsRegex.Matches( content );

        foreach ( Match match in matches )
        {
            string attributes = match.Groups["attrs"].Value;
            string componentTypesValue = ExtractAttributeValue( attributes, "ComponentTypes" );

            if ( !IsDynamicValue( componentTypesValue ) )
            {
                foreach ( string typeName in ExtractComponentTypes( componentTypesValue ) )
                {
                    if ( string.IsNullOrWhiteSpace( typeName ) )
                        continue;

                    DocsApiRef apiRef = new DocsApiRef
                    {
                        Kind = "type",
                        Name = typeName
                    };

                    if ( seen.Add( BuildApiRefKey( apiRef ) ) )
                    {
                        apiRefs.Add( apiRef );
                    }
                }
            }

            string category = ExtractAttributeValue( attributes, "Category" );

            if ( !IsDynamicValue( category ) )
            {
                string subcategory = ExtractAttributeValue( attributes, "Subcategory" );

                if ( IsDynamicValue( subcategory ) )
                    subcategory = null;

                DocsApiRef apiRef = new DocsApiRef
                {
                    Kind = "category",
                    Name = category,
                    Subcategory = subcategory
                };

                if ( seen.Add( BuildApiRefKey( apiRef ) ) )
                {
                    apiRefs.Add( apiRef );
                }
            }
        }

        return apiRefs;
    }

    private static IEnumerable<string> ExtractComponentTypes( string componentTypesValue )
    {
        if ( string.IsNullOrWhiteSpace( componentTypesValue ) )
            yield break;

        MatchCollection matches = ComponentTypeofRegex.Matches( componentTypesValue );

        foreach ( Match match in matches )
        {
            string rawType = match.Groups["type"].Value;
            string normalized = NormalizeComponentTypeName( rawType );

            if ( !string.IsNullOrWhiteSpace( normalized ) )
            {
                yield return normalized;
            }
        }
    }

    private static string NormalizeComponentTypeName( string rawType )
    {
        if ( string.IsNullOrWhiteSpace( rawType ) )
            return null;

        string trimmed = rawType.Trim();

        if ( trimmed.StartsWith( "global::", StringComparison.Ordinal ) )
            trimmed = trimmed.Substring( "global::".Length );

        int lastDot = trimmed.LastIndexOf( '.' );
        if ( lastDot >= 0 && lastDot + 1 < trimmed.Length )
            trimmed = trimmed.Substring( lastDot + 1 );

        int genericIndex = trimmed.IndexOf( '<' );
        if ( genericIndex >= 0 )
            trimmed = trimmed.Substring( 0, genericIndex );

        return trimmed.Trim();
    }

    private static string BuildApiRefKey( DocsApiRef apiRef )
    {
        string name = apiRef.Name ?? string.Empty;
        string subcategory = apiRef.Subcategory ?? string.Empty;
        return $"{apiRef.Kind}|{name}|{subcategory}";
    }

    private static ExampleMetadata ExtractSectionMetadata( string sectionContent )
    {
        string title = null;
        string description = null;

        Match headerMatch = SectionHeaderRegex.Match( sectionContent );
        if ( headerMatch.Success )
        {
            string attributes = headerMatch.Groups["attrs"].Value;
            title = ExtractAttributeValue( attributes, "Title" );
            description = NormalizeHeaderText( headerMatch.Groups["content"].Value );
        }
        else
        {
            Match selfClosingMatch = SectionHeaderSelfClosingRegex.Match( sectionContent );
            if ( selfClosingMatch.Success )
            {
                string attributes = selfClosingMatch.Groups["attrs"].Value;
                title = ExtractAttributeValue( attributes, "Title" );
            }
        }

        if ( IsDynamicValue( title ) )
            title = null;

        if ( string.IsNullOrWhiteSpace( description ) )
            description = null;

        if ( string.IsNullOrWhiteSpace( title ) && string.IsNullOrWhiteSpace( description ) )
            return null;

        return new ExampleMetadata( title, description );
    }

    private static string ExtractAttributeValue( string attributes, string attributeName )
    {
        if ( string.IsNullOrWhiteSpace( attributes ) )
            return null;

        MatchCollection matches = SeoAttributeRegex.Matches( attributes );

        foreach ( Match match in matches )
        {
            string name = match.Groups["name"].Value;

            if ( string.Equals( name, attributeName, StringComparison.OrdinalIgnoreCase ) )
            {
                string value = match.Groups["value"].Value;
                return UnescapeCSharpString( value );
            }
        }

        return null;
    }

    private static string NormalizeHeaderText( string content )
    {
        if ( string.IsNullOrWhiteSpace( content ) )
            return null;

        string cleaned = RazorCommentRegex.Replace( content, " " );
        cleaned = HtmlTagRegex.Replace( cleaned, " " );
        cleaned = cleaned.Replace( "\r", " " ).Replace( "\n", " " );
        cleaned = WhitespaceRegex.Replace( cleaned, " " ).Trim();

        return string.IsNullOrWhiteSpace( cleaned ) ? null : cleaned;
    }

    private static List<DocsExample> BuildExamples(
        string pageFilePath,
        List<string> exampleCodes,
        Dictionary<string, List<string>> exampleIndex,
        string docsRoot,
        Dictionary<string, ExampleMetadata> exampleMetadata )
    {
        List<DocsExample> examples = new List<DocsExample>();

        foreach ( string code in exampleCodes )
        {
            ExampleSource source = FindExampleSource( pageFilePath, code, exampleIndex );
            ExampleMetadata metadata = null;

            if ( exampleMetadata is not null )
            {
                exampleMetadata.TryGetValue( code, out metadata );
            }

            DocsExample example = new DocsExample
            {
                Code = code,
                Kind = source?.Kind,
                SourcePath = source is not null ? NormalizePath( Path.GetRelativePath( docsRoot, source.Path ) ) : null,
                Content = source is not null ? ReadExampleContent( source.Path ) : null,
                Title = metadata?.Title,
                Description = metadata?.Description
            };

            examples.Add( example );
        }

        return examples;
    }

    private static ExampleSource FindExampleSource( string pageFilePath, string code, Dictionary<string, List<string>> exampleIndex )
    {
        string pageDirectory = Path.GetDirectoryName( pageFilePath );
        string[] extensions = new[] { ".razor", ".snippet", ".csharp" };

        foreach ( string extension in extensions )
        {
            string localPath = Path.Combine( pageDirectory, "Examples", code + extension );

            if ( File.Exists( localPath ) )
                return new ExampleSource( localPath, GetKind( extension ) );
        }

        if ( exampleIndex.TryGetValue( code, out List<string> candidates ) && candidates.Count > 0 )
        {
            List<string> ordered = candidates.OrderBy( path => path, StringComparer.OrdinalIgnoreCase ).ToList();
            string selectedPath = ordered[0];
            return new ExampleSource( selectedPath, GetKind( Path.GetExtension( selectedPath ) ) );
        }

        return null;
    }

    private static string GetKind( string extension )
    {
        if ( extension.Equals( ".razor", StringComparison.OrdinalIgnoreCase ) )
            return "razor";
        if ( extension.Equals( ".snippet", StringComparison.OrdinalIgnoreCase ) )
            return "snippet";
        if ( extension.Equals( ".csharp", StringComparison.OrdinalIgnoreCase ) )
            return "csharp";

        return null;
    }

    private static string ReadExampleContent( string filePath )
    {
        string content = File.ReadAllText( filePath, Encoding.UTF8 );
        string cleaned = RazorDirectiveRegex.Replace( content, string.Empty );
        return cleaned.Trim();
    }

    private static string NormalizePath( string path )
    {
        return path.Replace( '\\', '/' );
    }

    private static ManualEntry GetManualEntry( Dictionary<string, ManualEntry> entries, string route )
    {
        if ( entries.TryGetValue( route, out ManualEntry entry ) )
            return entry;

        string normalized = route.StartsWith( "/", StringComparison.Ordinal ) ? route : "/" + route;

        if ( entries.TryGetValue( normalized, out ManualEntry normalizedEntry ) )
            return normalizedEntry;

        return null;
    }

    private static string UnescapeCSharpString( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return value;

        StringBuilder sb = new StringBuilder( value.Length );

        for ( int i = 0; i < value.Length; i++ )
        {
            char current = value[i];

            if ( current == '\\' && i + 1 < value.Length )
            {
                char next = value[i + 1];

                if ( next == '\\' || next == '"' )
                {
                    sb.Append( next );
                    i++;
                    continue;
                }

                if ( next == 'n' )
                {
                    sb.Append( '\n' );
                    i++;
                    continue;
                }

                if ( next == 'r' )
                {
                    sb.Append( '\r' );
                    i++;
                    continue;
                }

                if ( next == 't' )
                {
                    sb.Append( '\t' );
                    i++;
                    continue;
                }
            }

            sb.Append( current );
        }

        return sb.ToString();
    }

    private sealed class DocsIndex
    {
        public string GeneratedUtc { get; set; }
        public List<DocsPage> Pages { get; set; }
    }

    private sealed class DocsPage
    {
        public string Route { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PagePath { get; set; }
        public List<DocsExample> Examples { get; set; }
        public List<DocsApiRef> ApiRefs { get; set; }
    }

    private sealed class DocsExample
    {
        public string Code { get; set; }
        public string Kind { get; set; }
        public string SourcePath { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    private sealed class DocsApiRef
    {
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Subcategory { get; set; }
    }

    private sealed class ExampleMetadata
    {
        public ExampleMetadata( string title, string description )
        {
            Title = title;
            Description = description;
        }

        public string Title { get; }
        public string Description { get; }
    }

    private sealed class ManualEntry
    {
        public ManualEntry( string route, string name, string description )
        {
            Route = route;
            Name = name;
            Description = description;
        }

        public string Route { get; }
        public string Name { get; }
        public string Description { get; }
    }

    private sealed class SeoInfo
    {
        public SeoInfo( string title, string description )
        {
            Title = title;
            Description = description;
        }

        public string Title { get; }
        public string Description { get; }
    }

    private sealed class ExampleSource
    {
        public ExampleSource( string path, string kind )
        {
            Path = path;
            Kind = kind;
        }

        public string Path { get; }
        public string Kind { get; }
    }
}