using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ParameterRemovedAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZP003",
        title: "Blazorise parameter removed",
        messageFormat: "Parameter '{0}' was removed from component '{1}': {2}",
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
        public override void OnAttributeBeforeUpdate(
            OperationAnalysisContext context,
            MigrationContext migration,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            if ( component.Mapping is null )
                return;

            if ( !component.Mapping.ParameterRemovals.TryGetValue( attributeName, out var note ) )
                return;

            var properties = ImmutableDictionary<string, string?>.Empty
                .Add( MigrationDiagnosticProperties.OldName, attributeName );

            context.ReportDiagnostic( Diagnostic.Create(
                Rule,
                location,
                properties,
                attributeName,
                component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                note ) );
        }
    }
}
