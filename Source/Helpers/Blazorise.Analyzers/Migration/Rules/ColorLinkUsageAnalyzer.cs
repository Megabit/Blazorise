using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ColorLinkUsageAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZP004",
        title: "Blazorise Color.Link is button/alert-only",
        messageFormat: "Color.Link is only supported on Button or Alert components; '{0}' should use a different color",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction( compilationStart =>
        {
            INamedTypeSymbol? buttonType = compilationStart.Compilation.GetTypeByMetadataName( "Blazorise.Button" );
            if ( buttonType is null )
                return;

            INamedTypeSymbol? colorType = compilationStart.Compilation.GetTypeByMetadataName( "Blazorise.Color" );
            if ( colorType is null )
                return;

            INamedTypeSymbol? alertType = compilationStart.Compilation.GetTypeByMetadataName( "Blazorise.Alert" );

            IFieldSymbol? linkField = GetLinkField( colorType );
            if ( linkField is null )
                return;

            RenderTreeMigrationEngine.Register( compilationStart, new Handler( buttonType, alertType, linkField ), requireMapping: false );
        } );
    }

    private static IFieldSymbol? GetLinkField( INamedTypeSymbol colorType )
    {
        foreach ( ISymbol member in colorType.GetMembers( "Link" ) )
        {
            if ( member is IFieldSymbol field )
                return field;
        }

        return null;
    }

    private sealed class Handler : MigrationHandler
    {
        private readonly INamedTypeSymbol buttonType;
        private readonly INamedTypeSymbol? alertType;
        private readonly IFieldSymbol linkField;

        public Handler( INamedTypeSymbol buttonType, INamedTypeSymbol? alertType, IFieldSymbol linkField )
        {
            this.buttonType = buttonType;
            this.alertType = alertType;
            this.linkField = linkField;
        }

        public override void OnAttributeBeforeUpdate(
            OperationAnalysisContext context,
            MigrationContext migration,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            if ( !string.Equals( attributeName, "Color", System.StringComparison.Ordinal ) )
                return;

            if ( IsAllowedComponent( component.ComponentType, buttonType, alertType ) )
                return;

            if ( !ContainsColorLink( valueOperation, linkField ) )
                return;

            context.ReportDiagnostic( Diagnostic.Create(
                Rule,
                location,
                component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
        }

        private static bool IsAllowedComponent(
            INamedTypeSymbol componentType,
            INamedTypeSymbol buttonType,
            INamedTypeSymbol? alertType )
        {
            if ( IsComponentOrDerived( componentType, buttonType ) )
                return true;

            return alertType is not null && IsComponentOrDerived( componentType, alertType );
        }

        private static bool IsComponentOrDerived( INamedTypeSymbol componentType, INamedTypeSymbol candidateType )
        {
            for ( INamedTypeSymbol? current = componentType; current is not null; current = current.BaseType )
            {
                if ( SymbolEqualityComparer.Default.Equals( current, candidateType ) )
                    return true;
            }

            return false;
        }

        private static bool ContainsColorLink( IOperation? operation, IFieldSymbol linkField )
        {
            if ( operation is null )
                return false;

            Stack<IOperation> stack = new Stack<IOperation>();
            stack.Push( operation );

            while ( stack.Count > 0 )
            {
                IOperation current = stack.Pop();
                if ( current is IFieldReferenceOperation fieldReference
                     && SymbolEqualityComparer.Default.Equals( fieldReference.Field, linkField ) )
                {
                    return true;
                }

                foreach ( IOperation child in current.ChildOperations )
                    stack.Push( child );
            }

            return false;
        }
    }
}
