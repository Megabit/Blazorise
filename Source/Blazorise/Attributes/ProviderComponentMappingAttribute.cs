#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Declares a provider-specific component mapping so AOT tooling can materialize closed generic mappings at compile time.
/// </summary>
[AttributeUsage( AttributeTargets.Assembly, AllowMultiple = true )]
public sealed class ProviderComponentMappingAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderComponentMappingAttribute"/> class.
    /// </summary>
    /// <param name="componentType">The generic Blazorise component definition.</param>
    /// <param name="implementationType">The provider-specific component definition.</param>
    public ProviderComponentMappingAttribute( Type componentType, Type implementationType )
    {
        ComponentType = componentType;
        ImplementationType = implementationType;
    }

    /// <summary>
    /// Gets the generic Blazorise component definition.
    /// </summary>
    public Type ComponentType { get; }

    /// <summary>
    /// Gets the provider-specific component definition.
    /// </summary>
    public Type ImplementationType { get; }
}