using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazorise.ApiDocsGenerator.Helpers;


/// <summary>
/// Takes what is next to a property or a field.
/// </summary>
/// <param name="semanticModel"></param>
class FullyQualifiedNameRewriter( SemanticModel semanticModel ) : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        Logger.Log($"Visiting InvocationExpression: {node}");

        // Visit the expression being invoked (e.g., TimeSpan.FromMinutes)
        var newExpression = (ExpressionSyntax)Visit(node.Expression);

        // Visit the argument list
        var newArgumentList = (ArgumentListSyntax)Visit(node.ArgumentList);

        // Reconstruct the invocation expression with the updated expression and arguments
        var newInvocation = node.Update(newExpression, newArgumentList);

        Logger.Log($"Transformed InvocationExpression: {newInvocation}");

        return newInvocation.WithTriviaFrom(node);
    }

    public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        Logger.Log($"Visiting MemberAccessExpression: {node}");

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

            Logger.Log($"Replacing member access '{node}' with fully qualified name '{fullyQualifiedName}'");

            // Parse the fully qualified name into an expression
            var fullyQualifiedExpression = SyntaxFactory.ParseExpression(fullyQualifiedName)
                .WithTriviaFrom(node);

            return fullyQualifiedExpression;
        }
        else
        {
            // Reconstruct the member access expression with updated components
            var newMemberAccess = node.Update(newExpression, node.OperatorToken, newName);

            Logger.Log($"Transformed MemberAccessExpression: {newMemberAccess}");

            return newMemberAccess.WithTriviaFrom(node);
        }
    }

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        Logger.Log($"Visiting IdentifierName: {node.Identifier.Text}");

        var symbol = semanticModel.GetSymbolInfo(node).Symbol;

        if (symbol is INamedTypeSymbol typeSymbol)
        {
            // Fully qualify type names
            var fullyQualifiedName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            Logger.Log($"Replacing type identifier '{node}' with '{fullyQualifiedName}'");

            var qualifiedName = SyntaxFactory.ParseName(fullyQualifiedName)
                .WithTriviaFrom(node);

            return qualifiedName;
        }
        else if (symbol is IFieldSymbol or IPropertySymbol )
        {
            // Fully qualify field or property names
            var fullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            Logger.Log($"Replacing field/property identifier '{node}' with '{fullyQualifiedName}'");

            var qualifiedExpression = SyntaxFactory.ParseExpression(fullyQualifiedName)
                .WithTriviaFrom(node);

            return qualifiedExpression;
        }
        else
        {
            // Keep other identifiers (e.g., method names) as is
            Logger.Log($"Keeping identifier '{node}' as is");

            return node;
        }
    }


}