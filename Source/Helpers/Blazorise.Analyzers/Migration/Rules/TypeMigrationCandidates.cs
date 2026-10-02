#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#endregion

namespace Blazorise.Analyzers.Migration.Rules;

internal static class TypeMigrationCandidates
{
    internal static HashSet<string> CreateSimpleNames( IEnumerable<string> fullNames )
    {
        var names = new HashSet<string>( StringComparer.Ordinal );

        foreach ( var fullName in fullNames )
        {
            var simpleName = RenderTreeMigrationEngine.GetSimpleName( fullName );

            if ( simpleName is not null )
            {
                names.Add( simpleName );
            }
        }

        return names;
    }

    internal static bool ShouldAnalyze( SyntaxNode node, ISet<string> candidateSimpleNames )
    {
        var simpleName = node switch
        {
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            GenericNameSyntax genericName => genericName.Identifier.ValueText,
            _ => null,
        };

        return simpleName is not null && candidateSimpleNames.Contains( simpleName );
    }
}