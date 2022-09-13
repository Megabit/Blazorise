#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise
{
    /// <summary>
    /// Blazorise implementation of <see cref="IComponentActivator"/> used to initialize components that
    /// are registered through the dependency injection.
    /// </summary>
    public class ComponentActivator : IComponentActivator
    {
        #region Members

        private readonly IList<object> disposables;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to retrieve registered components.</param>
        public ComponentActivator( IServiceProvider serviceProvider )
        {
            ServiceProvider = serviceProvider;
            disposables = LoadServiceProviderDisposableList();
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
            RemoveDisposedComponents();

            var instance = ServiceProvider.GetService( componentType );

            if ( instance == null )
            {
                instance = Activator.CreateInstance( componentType );
            }

            if ( instance is not IComponent component )
            {
                throw new ArgumentException( $"The type {componentType.FullName} does not implement {nameof( IComponent )}.", nameof( componentType ) );
            }

            return component;
        }

        /// <summary>
        /// Loads the ServiceProvider's list of disposables
        /// </summary>
        /// <returns>List of object references the ServiceProvider uses to track disposables</returns>
        private IList<object> LoadServiceProviderDisposableList()
        {
            var disposablesPropertyInfo = ServiceProvider.GetType()
                .GetProperty( "Disposables", BindingFlags.Instance | BindingFlags.NonPublic );
            return disposablesPropertyInfo?.GetValue( ServiceProvider ) as IList<object>;
        }

        /// <summary>
        /// Removes all disposed Blazorise components from the ServiceProvider's disposables list.
        /// This prevents a memory leak where these references were being held permanently.
        /// </summary>
        private void RemoveDisposedComponents()
        {
            var disposedComponents = disposables
                .OfType<BaseAfterRenderComponent>()
                .Where( x => x.Disposed || x.AsyncDisposed )
                .ToList();

            foreach ( var component in disposedComponents )
            {
                disposables.Remove( component );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reference to the <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        #endregion
    }
}
