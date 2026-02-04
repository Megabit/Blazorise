using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Generator.Features;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.Generator
{
    [Generator]
    public sealed class EqualitySourceGenerator : IIncrementalGenerator
    {
        private const string STANDARD_SPACING = "\t\t\t";
        private const string USING_EXTENSIONS = "using Blazorise.Extensions;";

        public void Initialize( IncrementalGeneratorInitializationContext context )
        {
            // Find record declarations that have any attributes, then keep only those with [GenerateEquality]
            var records = context.SyntaxProvider.CreateSyntaxProvider(
                static ( node, _ ) => node is RecordDeclarationSyntax r && r.AttributeLists.Count > 0,
                static ( ctx, _ ) =>
                {
                    var record = (RecordDeclarationSyntax)ctx.Node;

                    // Quick syntax-level filter for [GenerateEquality] (no semantic model necessary for your current usage)
                    var attributeName = nameof( GenerateEqualityAttribute ).Replace( "Attribute", "" );

                    bool has = record.AttributeLists
                        .SelectMany( al => al.Attributes )
                        .Any( a =>
                        {
                            var name = a.Name switch
                            {
                                IdentifierNameSyntax id => id.Identifier.ValueText,
                                QualifiedNameSyntax qn => qn.Right.Identifier.ValueText,
                                AliasQualifiedNameSyntax an => an.Name.Identifier.ValueText,
                                _ => a.Name.ToString()
                            };

                            return string.Equals( name, attributeName, StringComparison.Ordinal )
                                || string.Equals( name, attributeName + "Attribute", StringComparison.Ordinal );
                        } );

                    return has ? record : null;
                } )
                .Where( static r => r is not null )!; // RecordDeclarationSyntax

            context.RegisterSourceOutput( records, static ( spc, recordObj ) =>
            {
                var record = (RecordDeclarationSyntax)recordObj!;
                AddGenerateEqualitySource( spc, record );
            } );
        }

        private static void AddGenerateEqualitySource( SourceProductionContext context, RecordDeclarationSyntax declarationSyntax )
        {
            var (namespaceName, usingsText) = GetNamespaceAndUsings( declarationSyntax );
            var className = declarationSyntax.Identifier.ValueText;

            var propertiesForEquality = new List<(string name, bool isCollection)>();

            foreach ( var item in declarationSyntax.ChildNodes() )
            {
                switch ( item )
                {
                    case PropertyDeclarationSyntax property:
                        if ( IsValid( property.AttributeLists ) )
                        {
                            if ( property.Type is GenericNameSyntax gns )
                            {
                                var genericTypeName = gns.Identifier.Text;
                                var collectionTypes = new[] { "IEnumerable", "List", "ICollection" };

                                if ( collectionTypes.Contains( genericTypeName ) )
                                    propertiesForEquality.Add( (property.Identifier.Text, true) );
                                else
                                    throw new Exception( $"Unhandled case!! Please check {nameof( EqualitySourceGenerator )}" );
                            }
                            else
                            {
                                propertiesForEquality.Add( (property.Identifier.Text, false) );
                            }
                        }
                        break;

                    case FieldDeclarationSyntax field:
                        if ( IsValid( field.AttributeLists ) )
                            propertiesForEquality.Add( (field.Declaration.Variables[0].Identifier.Text, false) );
                        break;

                    case EventFieldDeclarationSyntax eventDeclaration:
                        if ( IsValid( eventDeclaration.AttributeLists ) )
                            propertiesForEquality.Add( (eventDeclaration.Declaration.Variables[0].Identifier.Text, false) );
                        break;

                    default:
                        break;
                }
            }

            var returnEquals = GenerateEqualsReturn( propertiesForEquality );
            var returnGetHashCode = GenerateGetHashCodeReturn( propertiesForEquality );

            var src = $@"{usingsText}
{( usingsText.Contains( USING_EXTENSIONS ) ? string.Empty : USING_EXTENSIONS )}

namespace {namespaceName}
{{
    public partial record {className}
    {{
        /// <inheritdoc/>
        public virtual bool Equals({className} obj)
        {{
            {returnEquals}
        }}

        /// <inheritdoc/>
        public override int GetHashCode()
        {{
            {returnGetHashCode}
        }}
    }}
}}";

            context.AddSource( $"{className}.generated.cs", SourceText.From( src, Encoding.UTF8 ) );
        }

        private static bool IsValid( SyntaxList<AttributeListSyntax> attributes )
        {
            var attributeName = nameof( GenerateIgnoreEqualityAttribute ).Replace( "Attribute", "" );

            // Skip members that have [GenerateIgnoreEquality] or [GenerateIgnoreEqualityAttribute]
            return !attributes.Any( list =>
                list.Attributes.Any( attr =>
                {
                    var name = attr.Name switch
                    {
                        IdentifierNameSyntax id => id.Identifier.ValueText,
                        QualifiedNameSyntax qn => qn.Right.Identifier.ValueText,
                        AliasQualifiedNameSyntax an => an.Name.Identifier.ValueText,
                        _ => attr.Name.ToString()
                    };

                    return string.Equals( name, attributeName, StringComparison.Ordinal )
                        || string.Equals( name, $"{attributeName}Attribute", StringComparison.Ordinal );
                } ) );
        }

        private static string GenerateEqualsReturn( List<(string name, bool isCollection)> propertiesForEquality )
        {
            var sb = new StringBuilder();

            sb.AppendLine( "return obj is not null" );
            foreach ( (string prop, bool isCollection) in propertiesForEquality )
            {
                sb.Append( $"{STANDARD_SPACING}&& " );

                if ( isCollection )
                    sb.AppendLine( $"{prop}.AreEqual(obj.{prop})" );
                else
                    sb.AppendLine( $"{prop} == obj.{prop}" );
            }
            sb.Append( ";" );
            return sb.ToString();
        }

        private static string GenerateGetHashCodeReturn( List<(string name, bool isCollection)> propertiesForEquality )
        {
            var sb = new StringBuilder();
            sb.AppendLine( "var hash = new HashCode();" );

            foreach ( (string prop, bool isCollection) in propertiesForEquality )
            {
                if ( isCollection )
                {
                    sb.AppendLine( $"{STANDARD_SPACING}foreach (var hashItem in {prop})" );
                    sb.AppendLine( $"{STANDARD_SPACING}{{" );
                    sb.AppendLine( $"{STANDARD_SPACING}\thash.Add(hashItem);" );
                    sb.AppendLine( $"{STANDARD_SPACING}}}" );
                }
                else
                {
                    sb.AppendLine( $"{STANDARD_SPACING}hash.Add({prop});" );
                }
            }
            sb.Append( $"{STANDARD_SPACING}return hash.ToHashCode();" );
            return sb.ToString();
        }

        // --- Helpers to keep your original namespace/usings behavior, with support for file-scoped namespaces ---
        private static (string Namespace, string UsingsText) GetNamespaceAndUsings( RecordDeclarationSyntax record )
        {
            string ns = "Global";
            string usings = string.Empty;

            // Walk up to the compilation unit
            var cu = record.SyntaxTree.GetRoot() as CompilationUnitSyntax;
            if ( cu is not null )
            {
                usings = cu.Usings.ToString();
            }

            // Handle both block-scoped and file-scoped namespaces
            var parent = record.Parent;
            while ( parent is not null )
            {
                if ( parent is NamespaceDeclarationSyntax nds )
                {
                    ns = nds.Name.ToString();
                    break;
                }
                if ( parent is FileScopedNamespaceDeclarationSyntax fnds )
                {
                    ns = fnds.Name.ToString();
                    break;
                }
                parent = parent.Parent;
            }

            return (ns, usings);
        }
    }
}