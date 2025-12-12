using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration;

internal readonly struct MigrationContext
{
    public MigrationContext(
        Compilation compilation,
        IReadOnlyDictionary<string, ComponentMapping> componentByOld,
        IReadOnlyDictionary<string, ComponentMapping> componentByNew,
        IReadOnlyDictionary<string, string> tagByOld,
        ConcurrentDictionary<IMethodSymbol, TypeInferenceMapping?> typeInferenceMappings )
    {
        Compilation = compilation;
        ComponentByOld = componentByOld;
        ComponentByNew = componentByNew;
        TagByOld = tagByOld;
        TypeInferenceMappings = typeInferenceMappings;
    }

    public Compilation Compilation { get; }

    public IReadOnlyDictionary<string, ComponentMapping> ComponentByOld { get; }

    public IReadOnlyDictionary<string, ComponentMapping> ComponentByNew { get; }

    public IReadOnlyDictionary<string, string> TagByOld { get; }

    public ConcurrentDictionary<IMethodSymbol, TypeInferenceMapping?> TypeInferenceMappings { get; }
}

internal abstract class MigrationHandler
{
    public virtual void OnOpenComponent( OperationAnalysisContext context, MigrationContext migration, ComponentContext component ) { }

    public virtual void OnOpenElement( OperationAnalysisContext context, MigrationContext migration, string oldTag, string newTag, Location location ) { }

    public virtual void OnAttributeBeforeUpdate(
        OperationAnalysisContext context,
        MigrationContext migration,
        ComponentContext component,
        string attributeName,
        IOperation valueOperation,
        Location location ) { }

    public virtual void OnAttributeAfterUpdate(
        OperationAnalysisContext context,
        MigrationContext migration,
        ComponentContext component,
        string attributeName,
        IOperation valueOperation,
        Location location ) { }

    public virtual void OnCloseComponent( OperationAnalysisContext context, MigrationContext migration, ComponentContext component ) { }
}

internal sealed class ComponentContext
{
    public ComponentContext( INamedTypeSymbol componentType, ComponentMapping? mapping, Location componentLocation )
    {
        ComponentType = componentType;
        Mapping = mapping;
        ComponentLocation = componentLocation;
        TValueType = componentType.TypeArguments.Length > 0 ? componentType.TypeArguments[0] : null;
    }

    public INamedTypeSymbol ComponentType { get; }

    public ComponentMapping? Mapping { get; }

    public ITypeSymbol? TValueType { get; }

    public Location ComponentLocation { get; }

    public bool IsMultiple { get; set; }

    public ITypeSymbol? ValueType { get; set; }

    public Location? ValueLocation { get; set; }

    public bool HasReportedTValueShape { get; set; }

    public bool HasUnknownSelectionMode { get; set; }
}

internal readonly struct TypeInferenceAttributeMapping
{
    public TypeInferenceAttributeMapping( int valueParameterOrdinal, string attributeName )
    {
        ValueParameterOrdinal = valueParameterOrdinal;
        AttributeName = attributeName;
    }

    public int ValueParameterOrdinal { get; }

    public string AttributeName { get; }
}

internal sealed class TypeInferenceMapping
{
    public TypeInferenceMapping( INamedTypeSymbol componentType, IReadOnlyList<TypeInferenceAttributeMapping> attributes )
    {
        ComponentType = componentType;
        Attributes = attributes;
    }

    public INamedTypeSymbol ComponentType { get; }

    public IReadOnlyList<TypeInferenceAttributeMapping> Attributes { get; }
}

internal static class RenderTreeMigrationEngine
{
    public static void Register( CompilationStartAnalysisContext context, MigrationHandler handler )
    {
        var componentByOld = new Dictionary<string, ComponentMapping>( StringComparer.Ordinal );
        var componentByNew = new Dictionary<string, ComponentMapping>( StringComparer.Ordinal );
        var tagByOld = new Dictionary<string, string>( StringComparer.Ordinal );

        foreach ( var mapping in BlazoriseMigrationMappings.Components )
        {
            componentByOld[mapping.OldFullName] = mapping;
            if ( mapping.NewFullName is not null )
                componentByNew[mapping.NewFullName] = mapping;

            if ( mapping.NewFullName is not null
                 && !mapping.OldFullName.Equals( mapping.NewFullName, StringComparison.Ordinal ) )
            {
                var oldTag = GetSimpleName( mapping.OldFullName );
                var newTag = GetSimpleName( mapping.NewFullName );
                if ( oldTag is not null && newTag is not null && !tagByOld.ContainsKey( oldTag ) )
                    tagByOld[oldTag] = newTag;
            }
        }

        var typeInferenceMappings = new ConcurrentDictionary<IMethodSymbol, TypeInferenceMapping?>( SymbolEqualityComparer.Default );
        var migration = new MigrationContext( context.Compilation, componentByOld, componentByNew, tagByOld, typeInferenceMappings );

        context.RegisterOperationAction(
            ctx => AnalyzeBlock( ctx, migration, handler ),
            OperationKind.Block );
    }

    private static void AnalyzeBlock(
        OperationAnalysisContext context,
        MigrationContext migration,
        MigrationHandler handler )
    {
        if ( context.Operation is not IBlockOperation block )
            return;

        if ( context.ContainingSymbol is IMethodSymbol containingMethod
             && IsTypeInferenceHelperMethod( containingMethod ) )
            return;

        if ( !IsRootBlockForContainingSymbol( block, context.ContainingSymbol ) )
            return;

        var stack = new Stack<ComponentContext>();

        Traverse( block );

        void Traverse( IOperation operation )
        {
            if ( operation is IInvocationOperation invocation )
            {
                var target = invocation.TargetMethod;

                if ( TryHandleTypeInferenceInvocation( context, migration, handler, stack, invocation, target ) )
                {
                }
                else if ( target.Name.Equals( "OpenComponent", StringComparison.Ordinal ) )
                {
                    var componentType = GetComponentTypeFromOpenComponent( invocation, target );
                    if ( componentType is INamedTypeSymbol named )
                    {
                        var mapping = LookupComponentMapping( named, migration.ComponentByNew, migration.ComponentByOld );
                        var componentContext = new ComponentContext( named, mapping, invocation.Syntax.GetLocation() );
                        stack.Push( componentContext );
                        handler.OnOpenComponent( context, migration, componentContext );
                    }
                }
                else if ( target.Name.Equals( "OpenElement", StringComparison.Ordinal ) )
                {
                    if ( invocation.Arguments.Length >= 2 )
                    {
                        var tagNameConstant = UnwrapTypeCheck( invocation.Arguments[1].Value ).ConstantValue;
                        if ( tagNameConstant.HasValue
                             && tagNameConstant.Value is string tagName
                             && migration.TagByOld.TryGetValue( tagName, out var newTag ) )
                        {
                            handler.OnOpenElement(
                                context,
                                migration,
                                tagName,
                                newTag,
                                invocation.Arguments[1].Value.Syntax.GetLocation() );
                        }
                    }
                }
                else if ( target.Name.Equals( "CloseComponent", StringComparison.Ordinal ) )
                {
                    if ( stack.Count > 0 )
                    {
                        var current = stack.Peek();
                        handler.OnCloseComponent( context, migration, current );
                        stack.Pop();
                    }
                }
                else if ( target.Name is "AddAttribute" or "AddComponentParameter" )
                {
                    if ( stack.Count > 0 && target.Parameters.Length >= 3 && invocation.Arguments.Length >= 3 )
                    {
                        var attributeNameValue = UnwrapTypeCheck( invocation.Arguments[1].Value ).ConstantValue;
                        if ( attributeNameValue.HasValue && attributeNameValue.Value is string attributeName )
                        {
                            var currentComponent = stack.Peek();
                            var mapping = currentComponent.Mapping;
                            if ( mapping is not null )
                            {
                                var location = invocation.Arguments[1].Value.Syntax.GetLocation();
                                var valueOperation = invocation.Arguments[2].Value;

                                handler.OnAttributeBeforeUpdate( context, migration, currentComponent, attributeName, valueOperation, location );

                                UpdateMultipleFlag( attributeName, valueOperation, mapping, currentComponent );
                                UpdateValueBinding( attributeName, valueOperation, mapping, currentComponent );

                                handler.OnAttributeAfterUpdate( context, migration, currentComponent, attributeName, valueOperation, location );
                            }
                        }
                    }
                }
            }

            foreach ( var child in operation.ChildOperations )
                Traverse( child );
        }
    }

    private static bool IsRootBlockForContainingSymbol( IBlockOperation block, ISymbol? containingSymbol )
    {
        if ( containingSymbol is null )
            return true;

        if ( containingSymbol.DeclaringSyntaxReferences.Length == 0 )
            return true;

        foreach ( var syntaxRef in containingSymbol.DeclaringSyntaxReferences )
        {
            var syntax = syntaxRef.GetSyntax();

            switch ( syntax )
            {
                case MethodDeclarationSyntax methodDeclaration when methodDeclaration.Body is not null:
                    if ( methodDeclaration.Body == block.Syntax )
                        return true;
                    break;
                case LocalFunctionStatementSyntax localFunction when localFunction.Body is not null:
                    if ( localFunction.Body == block.Syntax )
                        return true;
                    break;
                case AccessorDeclarationSyntax accessorDeclaration when accessorDeclaration.Body is not null:
                    if ( accessorDeclaration.Body == block.Syntax )
                        return true;
                    break;
                case AnonymousFunctionExpressionSyntax anonymousFunction:
                    if ( anonymousFunction.Body is BlockSyntax anonymousBody && anonymousBody == block.Syntax )
                        return true;
                    break;
            }
        }

        return false;
    }

    private static bool IsTypeInferenceHelperMethod( IMethodSymbol method )
    {
        var containingType = method.ContainingType;
        return containingType is not null
               && containingType.Name.Equals( "TypeInference", StringComparison.Ordinal )
               && method.Name.StartsWith( "Create", StringComparison.Ordinal );
    }

    private static bool TryHandleTypeInferenceInvocation(
        OperationAnalysisContext context,
        MigrationContext migration,
        MigrationHandler handler,
        Stack<ComponentContext> stack,
        IInvocationOperation invocation,
        IMethodSymbol target )
    {
        if ( !IsTypeInferenceHelperMethod( target ) )
            return false;

        var mapping = GetOrCreateTypeInferenceMapping( target, migration );
        if ( mapping is null )
            return false;

        var componentType = CloseComponentType( mapping.ComponentType, target, migration.Compilation );
        var componentMapping = LookupComponentMapping( componentType, migration.ComponentByNew, migration.ComponentByOld );
        var componentContext = new ComponentContext( componentType, componentMapping, invocation.Syntax.GetLocation() );

        stack.Push( componentContext );
        handler.OnOpenComponent( context, migration, componentContext );

        if ( componentMapping is not null )
        {
            var argumentByOrdinal = new Dictionary<int, IOperation>();
            foreach ( var argument in invocation.Arguments )
            {
                if ( argument.Parameter is not null )
                    argumentByOrdinal[argument.Parameter.Ordinal] = argument.Value;
            }

            foreach ( var attribute in mapping.Attributes )
            {
                if ( !argumentByOrdinal.TryGetValue( attribute.ValueParameterOrdinal, out var valueOperation ) )
                    continue;

                var attributeName = attribute.AttributeName;
                var location = valueOperation.Syntax.GetLocation();

                handler.OnAttributeBeforeUpdate( context, migration, componentContext, attributeName, valueOperation, location );

                UpdateMultipleFlag( attributeName, valueOperation, componentMapping, componentContext );
                UpdateValueBinding( attributeName, valueOperation, componentMapping, componentContext );

                handler.OnAttributeAfterUpdate( context, migration, componentContext, attributeName, valueOperation, location );
            }
        }

        handler.OnCloseComponent( context, migration, componentContext );
        stack.Pop();

        return true;
    }

    private static TypeInferenceMapping? GetOrCreateTypeInferenceMapping( IMethodSymbol method, MigrationContext migration )
    {
        var originalDefinition = method.OriginalDefinition;

        if ( migration.TypeInferenceMappings.TryGetValue( originalDefinition, out var cached ) )
            return cached;

        var parsed = ParseTypeInferenceMapping( originalDefinition, migration );
        migration.TypeInferenceMappings.TryAdd( originalDefinition, parsed );
        return parsed;
    }

    private static TypeInferenceMapping? ParseTypeInferenceMapping( IMethodSymbol method, MigrationContext migration )
    {
        var syntaxRef = method.DeclaringSyntaxReferences.FirstOrDefault();
        if ( syntaxRef is null )
            return null;

        if ( syntaxRef.GetSyntax() is not MethodDeclarationSyntax methodDeclaration
             || methodDeclaration.Body is null )
            return null;

        var semanticModel = migration.Compilation.GetSemanticModel( methodDeclaration.SyntaxTree );
        if ( semanticModel.GetOperation( methodDeclaration.Body ) is not IBlockOperation methodBody )
            return null;

        INamedTypeSymbol? componentType = null;
        var attributes = new List<TypeInferenceAttributeMapping>();

        void Traverse( IOperation operation )
        {
            if ( operation is IInvocationOperation invocation )
            {
                var target = invocation.TargetMethod;

                if ( componentType is null && target.Name.Equals( "OpenComponent", StringComparison.Ordinal ) )
                {
                    var openType = GetComponentTypeFromOpenComponent( invocation, target );
                    if ( openType is INamedTypeSymbol named )
                        componentType = named;
                }

                if ( target.Name is "AddAttribute" or "AddComponentParameter" )
                {
                    if ( target.Parameters.Length >= 3 && invocation.Arguments.Length >= 3 )
                    {
                        var attributeNameValue = UnwrapTypeCheck( invocation.Arguments[1].Value ).ConstantValue;
                        if ( attributeNameValue.HasValue && attributeNameValue.Value is string attributeName )
                        {
                            var parameterReference = UnwrapParameterReference( invocation.Arguments[2].Value );
                            if ( parameterReference is not null )
                                attributes.Add( new TypeInferenceAttributeMapping( parameterReference.Parameter.Ordinal, attributeName ) );
                        }
                    }
                }
            }

            foreach ( var child in operation.ChildOperations )
                Traverse( child );
        }

        Traverse( methodBody );

        if ( componentType is null || attributes.Count == 0 )
            return null;

        return new TypeInferenceMapping( componentType, attributes );
    }

    private static IParameterReferenceOperation? UnwrapParameterReference( IOperation operation )
    {
        operation = UnwrapTypeCheck( operation );

        switch ( operation )
        {
            case IParameterReferenceOperation parameterReference:
                return parameterReference;
            case IConversionOperation conversion when conversion.Operand is not null:
                return UnwrapParameterReference( conversion.Operand );
            case IParenthesizedOperation parenthesized when parenthesized.Operand is not null:
                return UnwrapParameterReference( parenthesized.Operand );
            default:
                return null;
        }
    }

    private static INamedTypeSymbol CloseComponentType( INamedTypeSymbol componentType, IMethodSymbol typeInferenceMethod, Compilation compilation )
    {
        if ( typeInferenceMethod.TypeArguments.Length == 0 )
            return componentType;

        var originalDefinition = typeInferenceMethod.OriginalDefinition;
        if ( originalDefinition.TypeParameters.Length == 0 )
            return componentType;

        var substitution = new Dictionary<ITypeParameterSymbol, ITypeSymbol>( SymbolEqualityComparer.Default );
        var count = Math.Min( originalDefinition.TypeParameters.Length, typeInferenceMethod.TypeArguments.Length );
        for ( var i = 0; i < count; i++ )
        {
            var typeParameter = originalDefinition.TypeParameters[i];
            var typeArgument = typeInferenceMethod.TypeArguments[i];
            substitution[typeParameter] = typeArgument;
        }

        if ( substitution.Count == 0 )
            return componentType;

        return SubstituteMethodTypeParameters( componentType, substitution, compilation ) as INamedTypeSymbol ?? componentType;
    }

    private static ITypeSymbol SubstituteMethodTypeParameters(
        ITypeSymbol type,
        IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol> substitution,
        Compilation compilation )
    {
        switch ( type )
        {
            case ITypeParameterSymbol typeParameter when substitution.TryGetValue( typeParameter, out var replacement ):
                return replacement;
            case IArrayTypeSymbol arrayType:
                {
                    var elementType = SubstituteMethodTypeParameters( arrayType.ElementType, substitution, compilation );
                    return compilation.CreateArrayTypeSymbol( elementType, arrayType.Rank );
                }
            case INamedTypeSymbol namedType when namedType.IsGenericType:
                {
                    var substitutedArgs = namedType.TypeArguments
                        .Select( a => SubstituteMethodTypeParameters( a, substitution, compilation ) )
                        .ToArray();
                    return namedType.ConstructedFrom.Construct( substitutedArgs );
                }
            default:
                return type;
        }
    }

    internal static ComponentMapping? LookupComponentMapping(
        INamedTypeSymbol componentType,
        IReadOnlyDictionary<string, ComponentMapping> componentByNew,
        IReadOnlyDictionary<string, ComponentMapping> componentByOld )
    {
        var metadataName = GetMetadataName( componentType.ConstructedFrom );
        if ( componentByNew.TryGetValue( metadataName, out var mappingByNew ) )
            return mappingByNew;

        if ( componentByOld.TryGetValue( metadataName, out var mappingByOld ) )
            return mappingByOld;

        return null;
    }

    internal static string GetMetadataName( INamedTypeSymbol type )
    {
        var nsSymbol = type.ContainingNamespace;
        if ( nsSymbol is null || nsSymbol.IsGlobalNamespace )
            return type.MetadataName;

        var ns = nsSymbol.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );

        if ( string.IsNullOrEmpty( ns ) )
            return type.MetadataName;

        const string globalPrefix = "global::";
        if ( ns.StartsWith( globalPrefix, StringComparison.Ordinal ) )
            ns = ns.Substring( globalPrefix.Length );

        return $"{ns}.{type.MetadataName}";
    }

    private static ITypeSymbol? GetComponentTypeFromOpenComponent( IInvocationOperation invocation, IMethodSymbol target )
    {
        if ( target.TypeArguments.Length == 1 )
            return target.TypeArguments[0];

        if ( invocation.Arguments.Length >= 2 )
        {
            var typeOfOperand = UnwrapToTypeOfOperand( invocation.Arguments[1].Value );
            if ( typeOfOperand is not null )
                return typeOfOperand;
        }

        return null;
    }

    private static ITypeSymbol? UnwrapToTypeOfOperand( IOperation operation )
    {
        switch ( operation )
        {
            case ITypeOfOperation typeOfOperation:
                return typeOfOperation.TypeOperand;
            case IConversionOperation conversion when conversion.Operand is not null:
                return UnwrapToTypeOfOperand( conversion.Operand );
            case IParenthesizedOperation parenthesized when parenthesized.Operand is not null:
                return UnwrapToTypeOfOperand( parenthesized.Operand );
            default:
                return null;
        }
    }

    private static void UpdateValueBinding(
        string attributeName,
        IOperation valueOperation,
        ComponentMapping mapping,
        ComponentContext componentContext )
    {
        if ( string.Equals( attributeName, "Value", StringComparison.Ordinal )
             || ( mapping.ParameterRenames.TryGetValue( attributeName, out var newName )
                  && string.Equals( newName, "Value", StringComparison.Ordinal ) ) )
        {
            var unwrappedValueOperation = UnwrapTypeCheck( valueOperation );
            componentContext.ValueType = unwrappedValueOperation.Type;
            componentContext.ValueLocation = unwrappedValueOperation.Syntax.GetLocation();
        }
    }

    private static void UpdateMultipleFlag(
        string attributeName,
        IOperation valueOperation,
        ComponentMapping mapping,
        ComponentContext componentContext )
    {
        if ( string.Equals( attributeName, "Multiple", StringComparison.Ordinal )
             || ( mapping.ParameterRenames.TryGetValue( attributeName, out var newName )
                  && string.Equals( newName, "Multiple", StringComparison.Ordinal ) ) )
        {
            var constant = GetBooleanConstant( valueOperation );
            componentContext.IsMultiple |= constant is null || constant.Value;
            return;
        }

        if ( string.Equals( attributeName, "SelectionMode", StringComparison.Ordinal )
             || ( mapping.ParameterRenames.TryGetValue( attributeName, out var renamedSelectionMode )
                  && string.Equals( renamedSelectionMode, "SelectionMode", StringComparison.Ordinal ) ) )
        {
            var isMultiple = EvaluateSelectionMode( valueOperation );
            if ( isMultiple.HasValue )
            {
                componentContext.IsMultiple |= isMultiple.Value;
                componentContext.HasUnknownSelectionMode = false;
            }
            else
            {
                componentContext.HasUnknownSelectionMode = true;
            }
        }
    }

    private static bool? GetBooleanConstant( IOperation valueOperation )
    {
        valueOperation = UnwrapTypeCheck( valueOperation );

        switch ( valueOperation )
        {
            case IConversionOperation conversion when conversion.Operand is not null:
                return GetBooleanConstant( conversion.Operand );
            case IParenthesizedOperation parenthesized when parenthesized.Operand is not null:
                return GetBooleanConstant( parenthesized.Operand );
            case IUnaryOperation unary when unary.OperatorKind == UnaryOperatorKind.Not && unary.Operand is not null:
                {
                    var operand = GetBooleanConstant( unary.Operand );
                    return operand.HasValue ? !operand.Value : null;
                }
            default:
                var constantValue = valueOperation.ConstantValue;
                return constantValue.HasValue && constantValue.Value is bool boolValue ? boolValue : null;
        }
    }

    private static bool? EvaluateSelectionMode( IOperation valueOperation )
    {
        valueOperation = UnwrapTypeCheck( valueOperation );

        switch ( valueOperation )
        {
            case IFieldReferenceOperation fieldReference:
                {
                    if ( fieldReference.Field is IFieldSymbol field
                         && field.ContainingType?.TypeKind == TypeKind.Enum
                         && field.HasConstantValue )
                    {
                        if ( IsSingleName( field.Name ) )
                            return false;

                        if ( IsMultipleOrNonSingleName( field.Name ) )
                            return true;
                    }
                    break;
                }
            case IConversionOperation conversion:
                if ( conversion.Operand is not null )
                    return EvaluateSelectionMode( conversion.Operand );
                break;
        }

        if ( valueOperation.Type is INamedTypeSymbol enumType
             && enumType.TypeKind == TypeKind.Enum
             && valueOperation.ConstantValue.HasValue )
        {
            var constant = valueOperation.ConstantValue.Value;

            foreach ( var member in enumType.GetMembers().OfType<IFieldSymbol>() )
            {
                if ( member.HasConstantValue
                     && Equals( member.ConstantValue, constant )
                     && IsSingleName( member.Name ) )
                    return false;

                if ( member.HasConstantValue
                     && Equals( member.ConstantValue, constant )
                     && IsMultipleOrNonSingleName( member.Name ) )
                    return true;
            }

            var singleMember = enumType.GetMembers().OfType<IFieldSymbol>()
                .FirstOrDefault( x => x.HasConstantValue && string.Equals( x.Name, "Single", StringComparison.OrdinalIgnoreCase ) );

            if ( singleMember is not null && !Equals( singleMember.ConstantValue, constant ) )
                return true;

            return null;
        }

        return null;
    }

    private static IOperation UnwrapTypeCheck( IOperation operation )
    {
        switch ( operation )
        {
            case IConversionOperation conversion when conversion.Operand is not null:
                return UnwrapTypeCheck( conversion.Operand );
            case IParenthesizedOperation parenthesized when parenthesized.Operand is not null:
                return UnwrapTypeCheck( parenthesized.Operand );
            case IInvocationOperation invocation when invocation.Arguments.Length >= 1 && IsTypeCheckInvocation( invocation.TargetMethod ):
                return UnwrapTypeCheck( invocation.Arguments[0].Value );
            default:
                return operation;
        }
    }

    private static bool IsTypeCheckInvocation( IMethodSymbol method )
        => string.Equals( method.Name, "TypeCheck", StringComparison.Ordinal )
           && method.ContainingType is not null
           && string.Equals( method.ContainingType.Name, "RuntimeHelpers", StringComparison.Ordinal )
           && method.ContainingNamespace is not null
           && string.Equals( method.ContainingNamespace.ToDisplayString(), "Microsoft.AspNetCore.Components.CompilerServices", StringComparison.Ordinal );

    private static bool IsMultipleOrNonSingleName( string name )
    {
        if ( string.IsNullOrEmpty( name ) )
            return false;

        if ( name.IndexOf( "Multiple", StringComparison.OrdinalIgnoreCase ) >= 0 )
            return true;

        return !name.Equals( "Single", StringComparison.OrdinalIgnoreCase );
    }

    private static bool IsSingleName( string name )
        => string.Equals( name, "Single", StringComparison.OrdinalIgnoreCase );

    internal static bool IsMultiValueType( ITypeSymbol type )
    {
        if ( type.TypeKind == TypeKind.Array )
            return true;

        if ( type is INamedTypeSymbol named )
        {
            if ( named.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IReadOnlyList_T )
                return true;

            foreach ( var iface in named.AllInterfaces )
            {
                if ( iface.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IReadOnlyList_T )
                    return true;
            }
        }

        return false;
    }

    internal static ITypeSymbol? UnwrapNullable( ITypeSymbol? type )
    {
        if ( type is INamedTypeSymbol named
             && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
             && named.TypeArguments.Length == 1 )
        {
            return named.TypeArguments[0];
        }

        return type;
    }

    private static string? GetSimpleName( string fullName )
    {
        if ( string.IsNullOrWhiteSpace( fullName ) )
            return null;

        var tickIndex = fullName.IndexOf( '`' );
        var noArity = tickIndex >= 0 ? fullName.Substring( 0, tickIndex ) : fullName;
        var lastDot = noArity.LastIndexOf( '.' );
        return lastDot >= 0 ? noArity.Substring( lastDot + 1 ) : noArity;
    }
}
