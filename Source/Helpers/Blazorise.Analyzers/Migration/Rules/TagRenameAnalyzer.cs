using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class TagRenameAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZR001",
        title: "Blazorise Razor tag renamed",
        messageFormat: "Tag '{0}' was renamed to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction( compilationStart =>
        {
            RenderTreeMigrationEngine.Register( compilationStart, new Handler() );
        } );
    }

    private sealed class Handler : MigrationHandler
    {
        public override void OnOpenElement( OperationAnalysisContext context, MigrationContext migration, string oldTag, string newTag, Location location )
        {
            var properties = ImmutableDictionary<string, string?>.Empty
                .Add( MigrationDiagnosticProperties.OldName, oldTag )
                .Add( MigrationDiagnosticProperties.NewName, newTag );

            context.ReportDiagnostic( Diagnostic.Create(
                Rule,
                location,
                properties,
                oldTag,
                newTag ) );
        }
    }
}

