using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazorise.ApiDocsGenerator.Helpers;


/// <summary>
/// Used for retrieving default values of properties  
/// Takes what is next to a property or a field.
/// </summary>
/// <param name="semanticModel"></param>
class FullyQualifiedNameRewriter( SemanticModel semanticModel ) : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        // Visit the expression being invoked (e.g., TimeSpan.FromMinutes)
        var newExpression = (ExpressionSyntax)Visit(node.Expression);

        // Visit the argument list
        var newArgumentList = (ArgumentListSyntax)Visit(node.ArgumentList);

        // Reconstruct the invocation expression with the updated expression and arguments
        var newInvocation = node.Update(newExpression, newArgumentList);
        
        return newInvocation.WithTriviaFrom(node);
    }

    public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        // Visit the expression and name components
        var newExpression = (ExpressionSyntax)Visit(node.Expression);
        var newName = (SimpleNameSyntax)Visit(node.Name);

        // Get the symbol for the member access expression
        var symbol = semanticModel.GetSymbolInfo(node).Symbol;

        if (symbol != null)
        {
            var fullyQualifiedName = symbol.ToDisplayString(
            new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType
            )
            );
            // Parse the fully qualified name into an expression
            var fullyQualifiedExpression = SyntaxFactory.ParseExpression(fullyQualifiedName)
                .WithTriviaFrom(node);
        
            return fullyQualifiedExpression;
        }
        // Reconstruct the member access expression with updated components
        var newMemberAccess = node.Update(newExpression, node.OperatorToken, newName);

        return newMemberAccess.WithTriviaFrom(node);
    }

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        var symbol = semanticModel.GetSymbolInfo(node).Symbol;

        if (symbol is INamedTypeSymbol typeSymbol)
        {
            // Fully qualify type names
            var fullyQualifiedName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var qualifiedName = SyntaxFactory.ParseName(fullyQualifiedName)
                .WithTriviaFrom(node);

            return qualifiedName;
        }
        if (symbol is IFieldSymbol or IPropertySymbol )
        {
            // Fully qualify field or property names
            var fullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var qualifiedExpression = SyntaxFactory.ParseExpression(fullyQualifiedName)
                .WithTriviaFrom(node);

            return qualifiedExpression;
        }
        // Keep other identifiers (e.g., method names) as is
        return node;
    }


}