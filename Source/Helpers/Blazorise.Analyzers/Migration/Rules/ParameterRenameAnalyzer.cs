using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ParameterRenameAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZP001",
        title: "Blazorise parameter renamed",
        messageFormat: "Parameter '{0}' was renamed to '{1}' for component '{2}'",
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

            if ( component.Mapping.ParameterRenames.TryGetValue( attributeName, out var newName ) )
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName )
                    .Add( MigrationDiagnosticProperties.NewName, newName );

                context.ReportDiagnostic( Diagnostic.Create(
                    Rule,
                    location,
                    properties,
                    attributeName,
                    newName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
            }
        }
    }
}

