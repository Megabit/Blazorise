using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static Blazorise.Docs.Compiler.ExampleSources.ExampleSourceComposerHelpers;

namespace Blazorise.Docs.Compiler.ExampleSources;

internal sealed class ProviderExampleSourceComposer : IExampleSourceComposer
{
    private const string EmployeeSource = """
public class Employee
{
    public string FirstName { get; set; }

    public string Email { get; set; }
}
""";

    private static readonly string[] RequiredUsings =
    [
        "@using System",
        "@using System.Threading.Tasks"
    ];

    public bool CanHandle( string normalizedPath )
        => normalizedPath.Contains( "/Services/ModalProvider/Examples/", StringComparison.Ordinal )
            || normalizedPath.Contains( "/Services/OffcanvasProvider/Examples/", StringComparison.Ordinal );

    public string Prepare( string path, string source, ExampleSourceMode mode )
    {
        if ( mode != ExampleSourceMode.Copy
            || !Path.GetExtension( path ).Equals( ".razor", StringComparison.OrdinalIgnoreCase ) )
            return source;

        string filename = Path.GetFileName( path );

        return filename switch
        {
            "ModalProviderInstantiationExample.razor" or "ModalProviderStatefulExample.razor"
                or "OffcanvasProviderInstantiationExample.razor" or "OffcanvasProviderStatefulExample.razor"
                => InlineCounter( path, source ),
            "ModalProviderCustomStructureExample.razor" or "OffcanvasProviderCustomStructureExample.razor"
                or "OffcanvasServiceOptionsExample.razor"
                => InlineCustomStructure( path, source ),
            "ModalProviderFormularyExample.razor" or "OffcanvasProviderFormularyExample.razor"
                => InlineFormulary( path, source ),
            _ => source
        };
    }

    private static string InlineCounter( string path, string source )
    {
        string serviceName = ServiceName( path );
        string supportPath = SupportPath( path, "CounterExample.razor" );
        (string markup, string code) = ReadSupportComponent( supportPath );

        source = Regex.Replace(
            source,
            $"return {serviceName}[.]Show<CounterExample>\\(\\s*(?<title>\"[^\"]*\"),\\s*x => x[.]Add\\( x => x[.]Value, newValue \\)\\s*\\);",
            $"Value = newValue;{Environment.NewLine}        return {serviceName}.Show( ${{title}}, CounterExample );" );
        source = Regex.Replace(
            source,
            $"{serviceName}[.]Show<CounterExample>\\(\\s*(?<title>\"[^\"]*\"),\\s*new",
            $"{serviceName}.Show( ${{title}}, CounterExample, new" );

        code = Regex.Replace(
            code,
            @"(?m)^\s*\[Parameter\][ \t]+public long Value \{ get; set; \}\r?\n?",
            "private long Value { get; set; }" + Environment.NewLine );
        source = AddRequiredUsings( path, source, RequiredUsings );
        source = AppendFragment( path, source, "CounterExample", markup, code );
        ValidateComposedSource( path, source, ["Show<CounterExample>", "[Parameter]"] );

        return source;
    }

    private static string InlineCustomStructure( string path, string source )
    {
        bool isModal = path.Replace( '\\', '/' ).Contains( "/ModalProvider/", StringComparison.Ordinal );
        string serviceName = ServiceName( path );
        string componentName = isModal ? "CustomStructureModalExample" : "CustomStructureOffcanvasExample";
        string supportPath = SupportPath( path, $"{componentName}.razor" );
        (string markup, string code) = ReadSupportComponent( supportPath );

        if ( Path.GetFileName( path ).Equals( "OffcanvasServiceOptionsExample.razor", StringComparison.Ordinal ) )
        {
            source = Regex.Replace(
                source,
                $"{serviceName}[.]Show<{componentName}>\\(\\s*(?<title>\"[^\"]*\"),\\s*new",
                $"{serviceName}.Show( ${{title}}, {componentName}, new" );
        }
        else
        {
            source = Regex.Replace(
                source,
                $@"{serviceName}[.]Show<{componentName}>\(\s*parameters => parameters[.]Add\( x => x[.]UserName, userName \),\s*new",
                $"{serviceName}.Show( string.Empty, {componentName}, new" );
        }

        markup = Regex.Replace( markup, "@UserName\\b", "@userName" );
        code = Regex.Replace(
            code,
            $@"(?m)^\s*\[Inject\][ \t]+public I(?:Modal|Offcanvas)Service {serviceName} \{{ get; set; \}}\r?\n?",
            string.Empty );
        code = Regex.Replace(
            code,
            @"(?m)^\s*\[Parameter\][ \t]+public string UserName \{ get; set; \}\r?\n?",
            string.Empty );

        source = AddRequiredUsings( path, source, RequiredUsings );
        source = AppendFragment( path, source, componentName, markup, code );
        ValidateComposedSource( path, source, [$"Show<{componentName}>", "[Parameter]"] );

        return source;
    }

    private static string InlineFormulary( string path, string source )
    {
        bool isModal = path.Replace( '\\', '/' ).Contains( "/ModalProvider/", StringComparison.Ordinal );
        string serviceName = ServiceName( path );
        string componentName = isModal ? "FormularyModalExample" : "FormularyOffcanvasExample";
        string supportPath = SupportPath( path, $"{componentName}.razor" );
        (string markup, string code) = ReadSupportComponent( supportPath );

        source = Regex.Replace(
            source,
            $@"return {serviceName}[.]Show<{componentName}>\(\s*x =>\s*\{{.*?\}},\s*new",
            $"return {serviceName}.Show( string.Empty, {componentName}, new",
            RegexOptions.Singleline );

        code = Regex.Replace(
            code,
            $@"(?m)^\s*\[Inject\][ \t]+public I(?:Modal|Offcanvas)Service {serviceName} \{{ get; set; \}}\r?\n?",
            string.Empty );
        code = Regex.Replace(
            code,
            @"(?m)^\s*\[Parameter\][ \t]+public Func<Employee, Task(?:<bool>)?> On(?:Validate|Success) \{ get; set; \}\r?\n?",
            string.Empty );
        code = Regex.Replace(
            code,
            @"if \( OnValidate is not null \)\s*isValid = await OnValidate\( model \);",
            "isValid = await FormularyValidate( model );" );
        code = code.Replace( "await OnSuccess( model );", "await FormularySuccess( model );", StringComparison.Ordinal );
        code = $"{code.Trim()}{Environment.NewLine}{Environment.NewLine}{EmployeeSource.Trim()}";

        source = AddRequiredUsings( path, source, RequiredUsings );
        source = AppendFragment( path, source, componentName, markup, code );
        ValidateComposedSource( path, source, [$"Show<{componentName}>", "[Parameter]", "Blazorise.Shared"] );

        return source;
    }

    private static string ServiceName( string path )
        => path.Replace( '\\', '/' ).Contains( "/ModalProvider/", StringComparison.Ordinal )
            ? "ModalService"
            : "OffcanvasService";

    private static string SupportPath( string path, string filename )
    {
        string examplesDirectory = Path.GetDirectoryName( path );
        string supportPath = Path.Combine( examplesDirectory, filename );

        if ( File.Exists( supportPath ) )
            return supportPath;

        string servicesDirectory = Path.GetFullPath( Path.Combine( examplesDirectory, "..", ".." ) );
        supportPath = Path.Combine( servicesDirectory, "ModalProvider", "Examples", filename );

        if ( !File.Exists( supportPath ) )
            throw new InvalidOperationException( $"Provider example '{path}' is missing support component '{filename}'." );

        return supportPath;
    }

    private static (string Markup, string Code) ReadSupportComponent( string path )
    {
        string source = File.ReadAllText( path, Encoding.UTF8 );
        source = RemoveDocsDirectives( source );
        int codeBlockStart = source.IndexOf( "@code {", StringComparison.Ordinal );
        int codeBlockEnd = source.LastIndexOf( '}' );

        if ( codeBlockStart < 0 || codeBlockEnd <= codeBlockStart )
            throw new InvalidOperationException( $"Provider support component '{path}' must end with an @code block." );

        string markup = source[..codeBlockStart].Trim();
        string code = source[( codeBlockStart + "@code {".Length )..codeBlockEnd].Trim();
        return (markup, code);
    }

    private static string AppendFragment( string path, string source, string fragmentName, string markup, string code )
    {
        string fragment = $"private RenderFragment {fragmentName} => @<text>{Environment.NewLine}"
            + $"{IndentLines( markup, 4 )}{Environment.NewLine}"
            + $"</text>;{Environment.NewLine}{Environment.NewLine}{code.Trim()}";

        return AppendToCodeBlock( path, source, fragment );
    }
}