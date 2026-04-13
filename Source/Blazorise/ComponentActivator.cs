#region Using directives
using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise;

/// <summary>
/// Blazorise implementation of <see cref="IComponentActivator"/> used to initialize components that
/// are registered through the dependency injection.
/// </summary>
public class ComponentActivator : IComponentActivator
{
    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="serviceProvider">Service provider used to retrieve registered components.</param>
    public ComponentActivator( IServiceProvider serviceProvider )
    {
        ServiceProvider = serviceProvider;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Created the component for the specified type.
    /// </summary>
    /// <param name="componentType">Type of component to create.</param>
    /// <returns>Return the newly created component or raises an exception if the specified typo is invalid.</returns>
    public IComponent CreateInstance( Type componentType )
    {
        if ( TryCreateMappedInstance( componentType, out IComponent mappedComponent ) )
        {
            return mappedComponent;
        }

        var instance = ServiceProvider.GetService( componentType );

        if ( instance is null )
        {
            instance = ActivatorUtilities.CreateInstance( ServiceProvider, componentType );
        }

        if ( instance is not IComponent component )
        {
            throw new ArgumentException( $"The type {componentType.FullName} does not implement {nameof( IComponent )}.", nameof( componentType ) );
        }

        return component;
    }

    private bool TryCreateMappedInstance( Type componentType, out IComponent component )
    {
        component = null;

        if ( RuntimeFeature.IsDynamicCodeSupported
             || !componentType.IsConstructedGenericType
             || !ContainsValueTypeGenericArgument( componentType )
             || !GeneratedComponentMappingRegistry.TryResolve( componentType, out Type mappedType ) )
        {
            return false;
        }

        object instance = ActivatorUtilities.CreateInstance( ServiceProvider, mappedType );

        if ( instance is not IComponent resolvedComponent )
        {
            throw new ArgumentException( $"The type {mappedType.FullName} does not implement {nameof( IComponent )}.", nameof( componentType ) );
        }

        component = resolvedComponent;

        return true;
    }

    private static bool ContainsValueTypeGenericArgument( Type componentType )
    {
        foreach ( Type genericArgument in componentType.GetGenericArguments() )
        {
            if ( genericArgument.IsValueType )
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the <see cref="IServiceProvider"/>.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    #endregion
}