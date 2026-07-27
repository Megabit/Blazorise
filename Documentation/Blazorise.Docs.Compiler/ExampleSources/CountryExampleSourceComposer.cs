using System;
using System.IO;
using System.Text.RegularExpressions;
using static Blazorise.Docs.Compiler.ExampleSources.ExampleSourceComposerHelpers;

namespace Blazorise.Docs.Compiler.ExampleSources;

internal sealed class CountryExampleSourceComposer : IExampleSourceComposer
{
    private const string CountryDataInjectionPattern = @"(?m)^(?<indent>[ \t]*)\[Inject\]\r?\n[ \t]*public CountryData CountryData \{ get; set; \}";

    private const string PortableCountryDataSource = """
using System.Collections.Generic;
using System.Threading.Tasks;

public class CountryData
{
    public Task<IEnumerable<Country>> GetDataAsync()
        => Task.FromResult<IEnumerable<Country>>( new Country[]
        {
            new( "Croatia", "HR", "Zagreb" ),
            new( "France", "FR", "Paris" ),
            new( "Germany", "DE", "Berlin" ),
            new( "Italy", "IT", "Rome" ),
            new( "Japan", "JP", "Tokyo" ),
            new( "Portugal", "PT", "Lisbon" ),
            new( "Spain", "ES", "Madrid" ),
            new( "United Kingdom", "GB", "London" ),
            new( "United States", "US", "Washington, D.C." )
        } );
}
""";

    private static readonly string[] RequiredUsings =
    [
        "@using System",
        "@using System.Collections.Generic",
        "@using System.ComponentModel.DataAnnotations",
        "@using System.Linq",
        "@using System.Threading.Tasks"
    ];

    public bool CanHandle( string normalizedPath )
        => normalizedPath.Contains( "/Extensions/Autocomplete/Examples/", StringComparison.Ordinal )
            || normalizedPath.Contains( "/Extensions/DropdownList/Examples/", StringComparison.Ordinal )
            || normalizedPath.Contains( "/Extensions/ListView/Examples/", StringComparison.Ordinal );

    public string Prepare( string path, string source, ExampleSourceMode mode )
    {
        string filename = Path.GetFileName( path );

        if ( filename.Equals( "CountryData.csharp", StringComparison.OrdinalIgnoreCase ) )
            return PortableCountryDataSource;

        if ( filename.Equals( "Country.csharp", StringComparison.OrdinalIgnoreCase ) )
            return Regex.Replace( source, "(?m)^namespace .+;\\r?\\n", string.Empty );

        if ( !Path.GetExtension( path ).Equals( ".razor", StringComparison.OrdinalIgnoreCase )
            || !source.Contains( "CountryData", StringComparison.Ordinal ) )
            return source;

        source = Regex.Replace(
            source,
            CountryDataInjectionPattern,
            "${indent}private readonly CountryData CountryData = new();" );

        if ( mode == ExampleSourceMode.Copy )
        {
            source = AddRequiredUsings( path, source, RequiredUsings );

            string supportTypes = string.Join(
                Environment.NewLine + Environment.NewLine,
                ExtractSupportTypesFromSource( PortableCountryDataSource ),
                ExtractSupportTypes( SupportPath( path, "Country.csharp" ) ) );

            source = AppendToCodeBlock( path, source, supportTypes );
            ValidateComposedSource( path, source, ["Blazorise.Shared", "IMemoryCache", "GetManifestResourceStream", "[Inject]"] );
        }

        return source;
    }

    private static string SupportPath( string path, string filename )
    {
        string examplesDirectory = Path.GetDirectoryName( path );
        string supportPath = Path.Combine( examplesDirectory, filename );

        if ( File.Exists( supportPath ) )
            return supportPath;

        string extensionsDirectory = Path.GetFullPath( Path.Combine( examplesDirectory, "..", ".." ) );
        supportPath = Path.Combine( extensionsDirectory, "Autocomplete", "Examples", filename );

        if ( !File.Exists( supportPath ) )
            throw new InvalidOperationException( $"Country example '{path}' is missing support file '{filename}'." );

        return supportPath;
    }
}