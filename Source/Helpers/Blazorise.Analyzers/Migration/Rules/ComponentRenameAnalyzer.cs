using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ComponentRenameAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZC001",
        title: "Blazorise component renamed",
        messageFormat: "Component '{0}' was renamed to '{1}'",
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
        public override void OnOpenComponent( OperationAnalysisContext context, MigrationContext migration, ComponentContext component )
        {
            var metadataName = RenderTreeMigrationEngine.GetMetadataName( component.ComponentType.ConstructedFrom );

            if ( migration.ComponentByOld.TryGetValue( metadataName, out var mapping )
                 && mapping.NewFullName is not null
                 && !mapping.OldFullName.Equals( mapping.NewFullName, System.StringComparison.Ordinal ) )
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldFullName, mapping.OldFullName )
                    .Add( MigrationDiagnosticProperties.NewFullName, mapping.NewFullName );

                context.ReportDiagnostic( Diagnostic.Create(
                    Rule,
                    component.ComponentLocation,
                    properties,
                    mapping.OldFullName,
                    mapping.NewFullName ) );
            }
        }
    }
}

