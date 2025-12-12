using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Linq;
using Blazorise.Analyzers.Migration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

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
            new Blazorise.Analyzers.Migration.Rules.TValueShapeAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.MemberRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.MemberRemovedAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.TypeRenameAnalyzer(),
            new Blazorise.Analyzers.Migration.Rules.TypeRemovedAnalyzer(),
        };

        var diagnostics = await compilation
            .WithAnalyzers( ImmutableArray.Create( analyzers ) )
            .GetAnalyzerDiagnosticsAsync();

        return diagnostics;
    }
}
