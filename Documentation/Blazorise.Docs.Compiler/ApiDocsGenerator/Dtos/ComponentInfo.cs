using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Dtos;

public record ComponentInfo( INamedTypeSymbol Type, IEnumerable<IPropertySymbol> Properties, IEnumerable<IMethodSymbol> PublicMethods, IEnumerable<INamedTypeSymbol> InheritsFromChain )
{
    public INamedTypeSymbol Type { get; } = Type;
    public IEnumerable<IPropertySymbol> Properties { get; } = Properties;
    public IEnumerable<IMethodSymbol> PublicMethods { get; } = PublicMethods;
    public IEnumerable<INamedTypeSymbol> InheritsFromChain { get; } = InheritsFromChain;
}

