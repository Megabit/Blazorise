using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Blazorise.Docs.Compiler.ExampleSources.ExampleSourceComposerHelpers;

namespace Blazorise.Docs.Compiler.ExampleSources;

internal sealed class DataGridExampleSourceComposer : IExampleSourceComposer
{
    private const string PackageUsingPattern = "(?m)^@using Blazorise(?:[.]DataGrid|[.]Shared[.](?:Data|Models))?\\r?\\n";
    private const string EmployeeDataInjectionPattern = @"(?m)^(?<indent>[ \t]*)\[Inject\][ \t]+EmployeeData EmployeeData \{ get; set; \}";

    private static readonly string[] SupportFiles =
    [
        "DataGridEmployeeData.csharp",
        "DataGridEmployee.csharp",
        "DataGridSalary.csharp"
    ];

    private static readonly string[] RequiredUsings =
    [
        "@using System",
        "@using System.Collections.Generic",
        "@using System.ComponentModel.DataAnnotations",
        "@using System.Linq",
        "@using System.Threading.Tasks"
    ];

    public bool CanHandle( string normalizedPath )
        => normalizedPath.Contains( "/Extensions/DataGrid/Examples/", StringComparison.Ordinal );

    public string Prepare( string path, string source, ExampleSourceMode mode )
    {
        ValidateSource( path, source );

        if ( Path.GetExtension( path ).Equals( ".razor", StringComparison.OrdinalIgnoreCase ) )
        {
            source = Regex.Replace(
                source,
                EmployeeDataInjectionPattern,
                "${indent}private readonly EmployeeData EmployeeData = new();" );

            if ( mode == ExampleSourceMode.Copy && !source.Contains( CopyPasteReadyMarker, StringComparison.Ordinal ) )
            {
                source = InlineSupportTypes( path, source );
            }

            source = Regex.Replace( source, PackageUsingPattern, string.Empty );
        }
        else if ( mode == ExampleSourceMode.Display )
        {
            source = Regex.Replace( source, "(?m)^using Blazorise[.]Shared[.](?:Data|Models);\\r?\\n", string.Empty );
            source = Regex.Replace( source, "(?m)^namespace Blazorise[.]Shared[.](?:Data|Models);\\r?\\n", string.Empty );
        }

        return RemoveDocsDirectives( source );
    }

    private static string InlineSupportTypes( string path, string source )
    {
        source = AddRequiredUsings( path, source, RequiredUsings );

        string examplesDirectory = Path.GetDirectoryName( path );
        string supportTypes = string.Join(
            Environment.NewLine + Environment.NewLine,
            SupportFiles.Select( file => ExtractSupportTypes( Path.Combine( examplesDirectory, file ) ) ) );

        return AppendToCodeBlock( path, source, supportTypes );
    }

    private static void ValidateSource( string path, string source )
    {
        string[] docsOnlyDependencies =
        [
            "GetManifestResourceStream",
            "IMemoryCache"
        ];

        string docsOnlyDependency = docsOnlyDependencies.FirstOrDefault( dependency => source.Contains( dependency, StringComparison.Ordinal ) );

        if ( docsOnlyDependency is not null )
            throw new InvalidOperationException( $"DataGrid example source '{path}' references docs-only dependency '{docsOnlyDependency}'." );

        if ( source.Contains( "new EmployeeData", StringComparison.Ordinal ) )
            throw new InvalidOperationException( $"DataGrid example source '{path}' must inject the runtime EmployeeData service. The docs compiler makes the displayed source self-contained." );
    }
}