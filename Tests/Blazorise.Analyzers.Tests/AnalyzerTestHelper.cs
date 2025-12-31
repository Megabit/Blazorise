using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;
using System.Threading;
using Blazorise.Analyzers.Migration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.Analyzers.Tests;

internal static class AnalyzerTestHelper
{
    private const string FrameworkStubs = @"
namespace Microsoft.AspNetCore.Components
{
    public abstract class ComponentBase
    {
    }
}

namespace Microsoft.AspNetCore.Components.Rendering
{
    public class RenderTreeBuilder
    {
        public void OpenComponent<TComponent>(int sequence) { }
        public void OpenComponent(int sequence, System.Type componentType) { }
        public void CloseComponent() { }
        public void OpenElement(int sequence, string name) { }
        public void CloseElement() { }
        public void AddAttribute<TValue>(int sequence, string name, TValue value) { }
        public void AddComponentParameter<TValue>(int sequence, string name, TValue value) { }
    }
}
";

    public static Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync( string source )
        => GetDiagnosticsAsync( new[] { source } );

    public static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync( IEnumerable<string> sources )
        => await GetDiagnosticsAsync( sources, Enumerable.Empty<AdditionalText>() );

    public static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        IEnumerable<string> sources,
        IEnumerable<(string path, string content)> additionalFiles )
    {
        var additionalTexts = additionalFiles.Select( x => new InMemoryAdditionalText( x.path, x.content ) );
        return await GetDiagnosticsAsync( sources, additionalTexts );
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        IEnumerable<string> sources,
        IEnumerable<AdditionalText> additionalFiles )
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion( LanguageVersion.CSharp11 );

        var syntaxTrees = sources
            .Prepend( FrameworkStubs )
            .Select( source => CSharpSyntaxTree.ParseText( source, parseOptions ) )
            .ToArray();

        var references = new[]
        {
            MetadataReference.CreateFromFile( typeof( object ).GetTypeInfo().Assembly.Location ),
            MetadataReference.CreateFromFile( typeof( Enumerable ).GetTypeInfo().Assembly.Location ),
            MetadataReference.CreateFromFile( typeof( List<> ).GetTypeInfo().Assembly.Location ),
            MetadataReference.CreateFromFile( typeof( ImmutableArray ).GetTypeInfo().Assembly.Location ),
            MetadataReference.CreateFromFile( typeof( System.Runtime.CompilerServices.DynamicAttribute ).GetTypeInfo().Assembly.Location ),
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: syntaxTrees,
            references: references,
            options: new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary ) );

        DiagnosticAnalyzer[] analyzers =
        {
            new Blazorise.Analyzers.Migration.Rules.ComponentRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.TagRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.ParameterRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.ParameterTypeChangeAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.ParameterRemovedAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.TValueShapeAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.MemberRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.MemberRemovedAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.TypeRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.TypeRemovedAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.ChartJsStaticFilesAnalyzer(),
        };

        var options = new AnalyzerOptions( additionalFiles.ToImmutableArray() );

        var diagnostics = await compilation
            .WithAnalyzers( ImmutableArray.Create( analyzers ), options )
            .GetAnalyzerDiagnosticsAsync();

        return diagnostics;
    }

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText text;

        public InMemoryAdditionalText( string path, string content )
        {
            Path = path;
            text = SourceText.From( content );
        }

        public override string Path { get; }

        public override SourceText GetText( CancellationToken cancellationToken = default ) => text;
    }
}
