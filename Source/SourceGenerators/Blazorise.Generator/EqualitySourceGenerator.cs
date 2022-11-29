using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Generator.Features;
using System.Diagnostics.Contracts;

namespace Blazorise.Generator
{
    [Generator]
    public class EqualitySourceGenerator : ISourceGenerator
    {
        private const string STANDARD_SPACING = "\t\t\t";
        private const string USING_EXTENSIONS = "using Blazorise.Extensions;";

        public void Initialize( GeneratorInitializationContext context ) => context.RegisterForSyntaxNotifications( () => new GenerateEqualityFinder() );

        public void Execute( GeneratorExecutionContext context )
        {
            var records = ( (GenerateEqualityFinder)context.SyntaxReceiver ).Records;
            if ( records.Count > 0 )
            {
                foreach ( RecordDeclarationSyntax record in records )
                {
                    AddGenerateEqualitySource( context, record );
                }
            }
        }

        private void AddGenerateEqualitySource( GeneratorExecutionContext context, RecordDeclarationSyntax declarationSyntax )
        {
            var namespaceName = ( (Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax)declarationSyntax.Parent ).Name.ToString();
            var usings = ( (Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax)declarationSyntax.Parent.Parent ).Usings.ToString();
            var className = declarationSyntax.Identifier.ValueText;

            var propertiesForEquality = new List<(string name, bool isCollection)>();

            foreach ( var item in declarationSyntax.ChildNodes() )
            {
                switch ( item )
                {
                    case PropertyDeclarationSyntax property:
                        if ( IsValid( property.AttributeLists ) )
                        {
                            if ( property.Type is GenericNameSyntax )
                            {
                                var genericTypeName = ( (Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax)property.Type ).Identifier.Text;
                                var collectionTypes = new[] { "IEnumerable", "List", "ICollection" };

                                if ( collectionTypes.Contains( genericTypeName ) )
                                    propertiesForEquality.Add( (property.Identifier.Text, true) );
                                else
                                    throw new Exception( $"Unhandled case!! Please check {nameof( EqualitySourceGenerator )}" );
                            }
                            else
                                propertiesForEquality.Add( (property.Identifier.Text, false) );
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

            context.AddSource( $"{className}.generated.cs", SourceText.From( $@"{usings}
{( usings.Contains( USING_EXTENSIONS ) ? string.Empty : USING_EXTENSIONS )}

namespace {namespaceName}
{{
    public partial record {className} 
    {{

        /// <inheritdoc/>
        public virtual bool Equals( {className} obj )
        {{
            {returnEquals}
        }}

        /// <inheritdoc/>
        public override int GetHashCode()
        {{
            {returnGetHashCode}
        }}
    }}
}}", Encoding.UTF8 ) );
        }

        private bool IsValid( SyntaxList<AttributeListSyntax> attributes )
        {
            var attributeName = nameof( GenerateIgnoreEqualityAttribute ).Replace( "Attribute", "" );

            return !attributes
                    .Any( x => x.Attributes
                        .Any( y => ( (Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax)y.Name ).Identifier.ValueText == attributeName ) );
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
            sb.AppendLine( $"var hash = new HashCode();" );

            foreach ( (string prop, bool isCollection) in propertiesForEquality )
            {

                if ( isCollection )
                {
                    sb.AppendLine( $"{STANDARD_SPACING}foreach ( var hashItem in {prop} )" );
                    sb.AppendLine( $"{STANDARD_SPACING}{{" );
                    sb.AppendLine( $"{STANDARD_SPACING}\thash.Add( hashItem );" );
                    sb.AppendLine( $"{STANDARD_SPACING}}}" );
                }
                else
                    sb.AppendLine( $"{STANDARD_SPACING}hash.Add( {prop} );" );
            }
            sb.Append( $"{STANDARD_SPACING}return hash.ToHashCode();" );
            return sb.ToString();
        }

        private class GenerateEqualityFinder : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();
            public List<RecordDeclarationSyntax> Records { get; } = new List<RecordDeclarationSyntax>();

            public void OnVisitSyntaxNode( SyntaxNode syntaxNode )
            {
                var attributeName = nameof( GenerateEqualityAttribute ).Replace( "Attribute", "" );

                if ( syntaxNode is ClassDeclarationSyntax classDeclaration
                    && classDeclaration.AttributeLists
                    .Any( x => x.Attributes
                        .Any( y => ( (Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax)y.Name ).Identifier.ValueText == attributeName ) ) )
                {
                    Classes.Add( classDeclaration );
                }

                if ( syntaxNode is RecordDeclarationSyntax recordDeclaration
                    && recordDeclaration.AttributeLists
                    .Any( x => x.Attributes
                        .Any( y => ( (Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax)y.Name ).Identifier.ValueText == attributeName ) ) )
                {
                    Records.Add( recordDeclaration );
                }
            }
        }
    }
}
