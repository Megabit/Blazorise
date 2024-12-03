#region Using directives
using System;
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

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the <see cref="IServiceProvider"/>.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    #endregion
}