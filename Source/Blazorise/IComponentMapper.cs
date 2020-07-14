#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Map of all custom component implementations.
    /// </summary>
    public interface IComponentMapper
    {
        /// <summary>
        /// Gets the implementation component type.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        Type GetImplementation<TComponent>()
            where TComponent : IComponent;

        /// <summary>
        /// Gets the implementation component type.
        /// </summary>
        /// <param name="component">Base component.</param>
        /// <returns></returns>
        Type GetImplementation( IComponent component );

        /// <summary>
        /// Registers the implementation component to the base component.
        /// </summary>
        /// <typeparam name="TComponent">Base component type.</typeparam>
        /// <typeparam name="TImplementation">Implementation component type.</typeparam>
        void Register<TComponent, TImplementation>()
            where TComponent : IComponent
            where TImplementation : TComponent;

        /// <summary>
        /// Registers the implementation component types to the base component.
        /// </summary>
        /// <param name="component">Base component type.</param>
        /// <param name="implementation">Implementation component type.</param>
        void Register( Type component, Type implementation );

        /// <summary>
        /// Replaces already registered components with new custom implementation.
        /// </summary>
        /// <param name="component">Base component type.</param>
        /// <param name="implementation">Implementation component type.</param>
        void Replace( Type component, Type implementation );

        /// <summary>
        /// Checks if a component type has a custom registration.
        /// </summary>
        /// <typeparam name="TComponent">Component type to check.</typeparam>
        /// <returns>Returns true if the registration exists.</returns>
        bool HasRegistration<TComponent>()
            where TComponent : IComponent;

        /// <summary>
        /// Checks if a component has a custom implementation.
        /// </summary>
        /// <param name="component">Component to check.</param>
        /// <returns>Returns true if the registration exists.</returns>
        bool HasRegistration( IComponent component );
    }
}
