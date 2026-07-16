using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazorise.Docs.Compiler;

public class CodeSnippets
{
    private const string CopyPasteReadyMarker = "@* copy-paste-ready *@";
    private const string DataGridPackageUsingPattern = "(?m)^@using Blazorise(?:[.]DataGrid|[.]Shared[.](?:Data|Models))?\\r?\\n";
    private const string EmployeeDataInjectionPattern = @"(?m)^(?<indent>[ \t]*)\[Inject\][ \t]+EmployeeData EmployeeData \{ get; set; \}";
    private const string RazorDocsDirectivePattern = "@(namespace|layout|page) .+?\\r?\\n";

    private static readonly string[] DataGridSupportFiles =
    [
        "DataGridEmployeeData.csharp",
        "DataGridEmployee.csharp",
        "DataGridSalary.csharp"
    ];

    private static readonly string[] DataGridRequiredUsings =
    [
        "@using System",
        "@using System.Collections.Generic",
        "@using System.ComponentModel.DataAnnotations",
        "@using System.Linq",
        "@using System.Threading.Tasks"
    ];

    public bool Execute()
    {
        var success = true;
        try
        {
            var currentCode = string.Empty;

            if ( File.Exists( Paths.SnippetsFilePath() ) )
            {
                currentCode = File.ReadAllText( Paths.SnippetsFilePath() ).NormalizeGeneratedText();
            }

            var cb = new CodeBuilder();
            cb.AddHeader();
            cb.AddLine( $"namespace Blazorise.Docs.Models" );
            cb.AddLine( "{" );
            cb.IndentLevel++;
            cb.AddLine( $"public static partial class Snippets" );
            cb.AddLine( "{" );
            cb.IndentLevel++;

            var dirPath = Paths.DirPath();
            var razorFiles = Directory.EnumerateFiles( dirPath, "*.razor", SearchOption.AllDirectories );
            var snippetFiles = Directory.EnumerateFiles( dirPath, "*.snippet", SearchOption.AllDirectories );
            var csharpFiles = Directory.EnumerateFiles( dirPath, "*.csharp", SearchOption.AllDirectories );

            foreach ( var entry in razorFiles.Concat( snippetFiles ).Concat( csharpFiles ).OrderBy( e => e.Replace( "\\", "/" ), StringComparer.Ordinal ) )
            {
                var filename = Path.GetFileName( entry );
                var componentName = Path.GetFileNameWithoutExtension( filename );
                bool isCSharp = entry.EndsWith( ".csharp" );

                if ( !isCSharp && !componentName.Contains( Paths.ExampleDiscriminator ) )
                    continue;
                cb.AddLine( $"public const string {componentName} = @\"{EscapeComponentSource( entry )}\";" );
                cb.AddLine();
            }

            cb.IndentLevel--;
            cb.AddLine( "}" );
            cb.IndentLevel--;
            cb.AddLine( "}" );

            var builtCode = cb.ToString().NormalizeGeneratedText();

            if ( currentCode != builtCode )
            {
                File.WriteAllText( Paths.SnippetsFilePath(), builtCode );
            }
        }
        catch ( Exception e )
        {
            Console.WriteLine( $"Error generating {Paths.SnippetsFilePath} : {e.Message}" );
            success = false;
        }

        return success;
    }

    private static string EscapeComponentSource( string path )
    {
        var source = File.ReadAllText( path, Encoding.UTF8 );
        source = PrepareSourceForCopy( path, source );
        source = Regex.Replace( source, RazorDocsDirectivePattern, string.Empty );
        return source.Replace( "\"", "\"\"" ).Trim().ToCrLfLineEndings();
    }

    internal static string PrepareSourceForDisplay( string path, string source )
        => PrepareSource( path, source, false );

    internal static string PrepareSourceForCopy( string path, string source )
        => PrepareSource( path, source, true );

    private static string PrepareSource( string path, string source, bool composeFullExample )
    {
        bool isDataGridExample = path.Replace( '\\', '/' ).Contains( "/Extensions/DataGrid/Examples/", StringComparison.Ordinal );

        if ( isDataGridExample )
        {
            ValidateDataGridSource( path, source );

            if ( Path.GetExtension( path ).Equals( ".razor", StringComparison.OrdinalIgnoreCase ) )
            {
                source = Regex.Replace(
                    source,
                    EmployeeDataInjectionPattern,
                    "${indent}private readonly EmployeeData EmployeeData = new();" );

                if ( composeFullExample && !source.Contains( CopyPasteReadyMarker, StringComparison.Ordinal ) )
                {
                    source = InlineDataGridSupportTypes( path, source );
                }

                source = Regex.Replace( source, DataGridPackageUsingPattern, string.Empty );
            }
            else if ( !composeFullExample )
            {
                source = Regex.Replace( source, "(?m)^using Blazorise[.]Shared[.](?:Data|Models);\\r?\\n", string.Empty );
                source = Regex.Replace( source, "(?m)^namespace Blazorise[.]Shared[.](?:Data|Models);\\r?\\n", string.Empty );
            }

            source = Regex.Replace( source, RazorDocsDirectivePattern, string.Empty );
        }

        if ( source.Contains( CopyPasteReadyMarker, StringComparison.Ordinal ) )
        {
            ValidateCopyPasteReadySource( path, source );
            source = Regex.Replace( source, $"{Regex.Escape( CopyPasteReadyMarker )}\\r?\\n", string.Empty );
        }

        return source;
    }

    private static string InlineDataGridSupportTypes( string path, string source )
    {
        string[] missingUsings = DataGridRequiredUsings
            .Where( requiredUsing => !Regex.IsMatch( source, $"(?m)^{Regex.Escape( requiredUsing )}\\r?$" ) )
            .ToArray();

        if ( missingUsings.Length > 0 )
        {
            Match namespaceDirective = Regex.Match( source, "@namespace[^\\r\\n]+\\r?\\n" );

            if ( !namespaceDirective.Success )
                throw new InvalidOperationException( $"DataGrid example '{path}' must declare a namespace before its copy-ready imports can be composed." );

            string imports = string.Join( Environment.NewLine, missingUsings ) + Environment.NewLine;
            source = source.Insert( namespaceDirective.Index + namespaceDirective.Length, imports );
        }

        string examplesDirectory = Path.GetDirectoryName( path );
        string supportTypes = string.Join(
            Environment.NewLine + Environment.NewLine,
            DataGridSupportFiles.Select( file => ExtractSupportTypes( Path.Combine( examplesDirectory, file ) ) ) );

        int codeBlockEnd = source.LastIndexOf( '}' );

        if ( codeBlockEnd < 0 || !source.Contains( "@code", StringComparison.Ordinal ) )
            throw new InvalidOperationException( $"DataGrid example '{path}' must end with an @code block so support types can be inlined." );

        string indentedSupportTypes = string.Join(
            Environment.NewLine,
            supportTypes.Split( ["\r\n", "\n"], StringSplitOptions.None ).Select( line => $"    {line}" ) );

        return source.Insert( codeBlockEnd, $"{Environment.NewLine}{Environment.NewLine}{indentedSupportTypes}{Environment.NewLine}" );
    }

    private static string ExtractSupportTypes( string path )
    {
        string source = File.ReadAllText( path, Encoding.UTF8 );
        source = Regex.Replace( source, "(?m)^using .+;\\r?\\n", string.Empty );
        source = Regex.Replace( source, "(?m)^namespace .+;\\r?\\n", string.Empty );
        return source.Trim();
    }

    private static void ValidateCopyPasteReadySource( string path, string source )
    {
        string[] docsOnlyDependencies =
        [
            "@inject ",
            "[Inject]",
            "Blazorise.Shared",
            "EmployeeData"
        ];

        string docsOnlyDependency = docsOnlyDependencies.FirstOrDefault( dependency => source.Contains( dependency, StringComparison.Ordinal ) );

        if ( docsOnlyDependency is not null )
            throw new InvalidOperationException( $"Copy-paste-ready example '{path}' references docs-only dependency '{docsOnlyDependency}'." );

        if ( !source.Contains( "@code", StringComparison.Ordinal ) )
            throw new InvalidOperationException( $"Copy-paste-ready example '{path}' must include its component state in an @code block." );
    }

    private static void ValidateDataGridSource( string path, string source )
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