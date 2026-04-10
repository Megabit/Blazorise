#region Using directives
using System;
using System.Collections.Concurrent;
#endregion

namespace Blazorise;

/// <summary>
/// Stores generated closed component mappings for Native AOT scenarios.
/// </summary>
public static class GeneratedComponentMappingRegistry
{
    private static readonly ConcurrentDictionary<Type, Type> mappings = new();

    /// <summary>
    /// Registers a generated component mapping.
    /// </summary>
    /// <param name="componentType">The requested component type.</param>
    /// <param name="implementationType">The provider-specific implementation type.</param>
    public static void Register( Type componentType, Type implementationType )
    {
        mappings[componentType] = implementationType;
    }

    /// <summary>
    /// Attempts to resolve a generated component mapping.
    /// </summary>
    /// <param name="componentType">The requested component type.</param>
    /// <param name="implementationType">The resolved provider-specific implementation type.</param>
    /// <returns>True if a generated mapping exists; otherwise false.</returns>
    public static bool TryResolve( Type componentType, out Type implementationType )
    {
        return mappings.TryGetValue( componentType, out implementationType );
    }
}