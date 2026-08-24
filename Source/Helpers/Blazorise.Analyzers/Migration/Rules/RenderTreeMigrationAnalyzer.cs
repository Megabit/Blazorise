using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class RenderTreeMigrationAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor ComponentRenameRule = new(
        id: "BLZC001",
        title: "Blazorise component renamed",
        messageFormat: "Component '{0}' was renamed to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor TagRenameRule = new(
        id: "BLZR001",
        title: "Blazorise Razor tag renamed",
        messageFormat: "Tag '{0}' was renamed to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ParameterRenameRule = new(
        id: "BLZP001",
        title: "Blazorise parameter renamed",
        messageFormat: "Parameter '{0}' was renamed to '{1}' for component '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ParameterTypeChangeRule = new(
        id: "BLZP002",
        title: "Blazorise parameter type changed",
        messageFormat: "Parameter '{0}' on component '{1}' has changed type: {2}",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ParameterRemovedRule = new(
        id: "BLZP003",
        title: "Blazorise parameter removed",
        messageFormat: "Parameter '{0}' was removed from component '{1}': {2}",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ParameterObsoleteRule = new(
        id: "BLZP005",
        title: "Blazorise parameter obsolete",
        messageFormat: "Parameter '{0}' is obsolete on component '{1}': {2}",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ColorLinkRule = new(
        id: "BLZP004",
        title: "Blazorise Color.Link is button/alert-only",
        messageFormat: "Color.Link is only supported on Button or Alert components; '{0}' should use a different color",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor TValueShapeRule = new(
        id: "BLZT001",
        title: "Blazorise TValue shape is invalid",
        messageFormat: "Component '{0}' expects TValue shape '{1}', but value expression is of type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ValidationFeedbackPlacementRule = new(
        id: "BLZV001",
        title: "Blazorise validation feedback placement changed",
        messageFormat: "Validation feedback component '{0}' should be placed inside the input's 'Feedback' content instead of directly inside 'Addons'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        ComponentRenameRule,
        TagRenameRule,
        ParameterRenameRule,
        ParameterTypeChangeRule,
        ParameterRemovedRule,
        ParameterObsoleteRule,
        ColorLinkRule,
        TValueShapeRule,
        ValidationFeedbackPlacementRule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction( compilationStart =>
        {
            if ( compilationStart.Compilation.GetTypeByMetadataName( "Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder" ) is null )
                return;

            ColorLinkState? colorLinkState = CreateColorLinkState( compilationStart.Compilation );
            RenderTreeMigrationEngine.Register( compilationStart, new Handler( colorLinkState ), requireMapping: false );
        } );
    }

    private static ColorLinkState? CreateColorLinkState( Compilation compilation )
    {
        INamedTypeSymbol? buttonType = compilation.GetTypeByMetadataName( "Blazorise.Button" );
        if ( buttonType is null )
            return null;

        INamedTypeSymbol? colorType = compilation.GetTypeByMetadataName( "Blazorise.Color" );
        if ( colorType is null )
            return null;

        IFieldSymbol? linkField = GetLinkField( colorType );
        if ( linkField is null )
            return null;

        INamedTypeSymbol? alertType = compilation.GetTypeByMetadataName( "Blazorise.Alert" );
        return new ColorLinkState( buttonType, alertType, linkField );
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
        private readonly ColorLinkState? colorLinkState;

        public Handler( ColorLinkState? colorLinkState )
        {
            this.colorLinkState = colorLinkState;
        }

        public override void OnOpenComponent(
            OperationAnalysisContext context,
            MigrationContext migration,
            ComponentContext component,
            ComponentContext? parentComponent )
        {
            string metadataName = RenderTreeMigrationEngine.GetMetadataName( component.ComponentType.ConstructedFrom );

            ReportValidationFeedbackPlacement( context, component, parentComponent, metadataName );

            if ( migration.ComponentByOld.TryGetValue( metadataName, out ComponentMapping mapping )
                 && mapping.NewFullName is not null
                 && !mapping.OldFullName.Equals( mapping.NewFullName, StringComparison.Ordinal ) )
            {
                ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldFullName, mapping.OldFullName )
                    .Add( MigrationDiagnosticProperties.NewFullName, mapping.NewFullName );

                context.ReportDiagnostic( Diagnostic.Create(
                    ComponentRenameRule,
                    component.ComponentLocation,
                    properties,
                    mapping.OldFullName,
                    mapping.NewFullName ) );
            }
        }

        private static void ReportValidationFeedbackPlacement(
            OperationAnalysisContext context,
            ComponentContext component,
            ComponentContext? parentComponent,
            string metadataName )
        {
            if ( parentComponent is null
                 || !string.Equals(
                     RenderTreeMigrationEngine.GetMetadataName( parentComponent.ComponentType.ConstructedFrom ),
                     "Blazorise.Addons",
                     StringComparison.Ordinal )
                 || !IsValidationFeedbackComponent( metadataName ) )
            {
                return;
            }

            context.ReportDiagnostic( Diagnostic.Create(
                ValidationFeedbackPlacementRule,
                component.ComponentLocation,
                component.ComponentType.Name ) );
        }

        private static bool IsValidationFeedbackComponent( string metadataName )
            => metadataName is "Blazorise.ValidationNone"
                or "Blazorise.ValidationSuccess"
                or "Blazorise.ValidationWarning"
                or "Blazorise.ValidationError"
                or "Blazorise.ValidationFeedback";

        public override void OnOpenElement(
            OperationAnalysisContext context,
            MigrationContext migration,
            string oldTag,
            string newTag,
            Location location )
        {
            ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                .Add( MigrationDiagnosticProperties.OldName, oldTag )
                .Add( MigrationDiagnosticProperties.NewName, newTag );

            context.ReportDiagnostic( Diagnostic.Create(
                TagRenameRule,
                location,
                properties,
                oldTag,
                newTag ) );
        }

        public override void OnAttributeBeforeUpdate(
            OperationAnalysisContext context,
            MigrationContext migration,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            ReportColorLinkUsage( context, component, attributeName, valueOperation, location );

            ComponentMapping? mapping = component.Mapping;
            if ( mapping is null )
                return;

            if ( mapping.ParameterRenames.TryGetValue( attributeName, out string newName ) )
            {
                ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName )
                    .Add( MigrationDiagnosticProperties.NewName, newName );

                context.ReportDiagnostic( Diagnostic.Create(
                    ParameterRenameRule,
                    location,
                    properties,
                    attributeName,
                    newName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
            }

            if ( mapping.ParameterRemovals.TryGetValue( attributeName, out string removalNote ) )
            {
                ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName );

                context.ReportDiagnostic( Diagnostic.Create(
                    ParameterRemovedRule,
                    location,
                    properties,
                    attributeName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    removalNote ) );
            }

            if ( mapping.ParameterObsoleteMessages.TryGetValue( attributeName, out string obsoleteMessage ) )
            {
                ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName );

                context.ReportDiagnostic( Diagnostic.Create(
                    ParameterObsoleteRule,
                    location,
                    properties,
                    attributeName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    obsoleteMessage ) );
            }

            ReportParameterTypeChanges( context, component, attributeName, valueOperation, location );
        }

        public override void OnCloseComponent( OperationAnalysisContext context, MigrationContext migration, ComponentContext component )
        {
            ReportTValueShape( context, component );
        }

        private static void ReportParameterTypeChanges(
            OperationAnalysisContext context,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            string metadataName = RenderTreeMigrationEngine.GetMetadataName( component.ComponentType.ConstructedFrom );
            ITypeSymbol? valueType = RenderTreeMigrationEngine.UnwrapNullable( valueOperation.Type );

            if ( ( string.Equals( metadataName, "Blazorise.DataGrid.DataGridColumn`1", StringComparison.Ordinal )
                   || string.Equals( metadataName, "Blazorise.DataGridColumn`1", StringComparison.Ordinal ) )
                 && string.Equals( attributeName, "Width", StringComparison.Ordinal )
                 && valueType?.SpecialType == SpecialType.System_String )
            {
                ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName );

                context.ReportDiagnostic( Diagnostic.Create(
                    ParameterTypeChangeRule,
                    location,
                    properties,
                    attributeName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    "Width now expects IFluentSizing (e.g., Width.Px(60))." ) );
            }

            if ( ( string.Equals( metadataName, "Blazorise.Row", StringComparison.Ordinal )
                   || string.Equals( metadataName, "Blazorise.Fields", StringComparison.Ordinal ) )
                 && string.Equals( attributeName, "Gutter", StringComparison.Ordinal )
                 && valueType is INamedTypeSymbol named
                 && named.IsTupleType )
            {
                ImmutableDictionary<string, string?> properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName );

                context.ReportDiagnostic( Diagnostic.Create(
                    ParameterTypeChangeRule,
                    location,
                    properties,
                    attributeName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    "Gutter now expects IFluentGutter fluent API instead of tuple." ) );
            }
        }

        private void ReportColorLinkUsage(
            OperationAnalysisContext context,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            if ( colorLinkState is null )
                return;

            if ( !string.Equals( attributeName, "Color", StringComparison.Ordinal ) )
                return;

            if ( IsAllowedComponent( component.ComponentType, colorLinkState.ButtonType, colorLinkState.AlertType ) )
                return;

            if ( !ContainsColorLink( valueOperation, colorLinkState.LinkField ) )
                return;

            context.ReportDiagnostic( Diagnostic.Create(
                ColorLinkRule,
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

        private static void ReportTValueShape( OperationAnalysisContext context, ComponentContext componentContext )
        {
            ComponentMapping? mapping = componentContext.Mapping;

            if ( mapping is null || mapping.TValueShape == TValueShape.Any )
                return;

            if ( componentContext.HasReportedTValueShape )
                return;

            ITypeSymbol? valueType = componentContext.ValueType ?? componentContext.TValueType;
            Location? location = componentContext.ValueLocation ?? componentContext.ComponentLocation;

            if ( valueType is null || location is null )
                return;

            if ( mapping.TValueShape == TValueShape.Single && RenderTreeMigrationEngine.IsMultiValueType( valueType ) )
            {
                context.ReportDiagnostic( Diagnostic.Create(
                    TValueShapeRule,
                    location,
                    componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    mapping.TValueShape.ToString(),
                    valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
                componentContext.HasReportedTValueShape = true;
                return;
            }

            ITypeSymbol? multiShapeType = componentContext.ValueType ?? componentContext.TValueType ?? valueType;

            if ( mapping.TValueShape == TValueShape.SingleOrMultiListOrArray
                 && componentContext.IsMultiple
                 && multiShapeType is not null
                 && !RenderTreeMigrationEngine.IsMultiValueType( multiShapeType ) )
            {
                context.ReportDiagnostic( Diagnostic.Create(
                    TValueShapeRule,
                    location,
                    componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    TValueShape.SingleOrMultiListOrArray.ToString(),
                    valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
                componentContext.HasReportedTValueShape = true;
            }

            if ( mapping.TValueShape == TValueShape.SingleOrMultiListOrArray
                 && !componentContext.IsMultiple
                 && !componentContext.HasUnknownSelectionMode
                 && multiShapeType is not null
                 && RenderTreeMigrationEngine.IsMultiValueType( multiShapeType ) )
            {
                context.ReportDiagnostic( Diagnostic.Create(
                    TValueShapeRule,
                    location,
                    componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    TValueShape.Single.ToString(),
                    multiShapeType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
                componentContext.HasReportedTValueShape = true;
            }
        }
    }

    private sealed class ColorLinkState
    {
        public ColorLinkState( INamedTypeSymbol buttonType, INamedTypeSymbol? alertType, IFieldSymbol linkField )
        {
            ButtonType = buttonType;
            AlertType = alertType;
            LinkField = linkField;
        }

        public INamedTypeSymbol ButtonType { get; }

        public INamedTypeSymbol? AlertType { get; }

        public IFieldSymbol LinkField { get; }
    }
}