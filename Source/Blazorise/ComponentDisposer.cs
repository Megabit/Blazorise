#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Handles the disposable transient components by freeing them from memory.
    /// </summary>
    public interface IComponentDisposer
    {
        /// <summary>
        /// Frees the component from <see cref="IServiceProvider"/> disposable collection.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component to free.</typeparam>
        /// <param name="component">Component to free.</param>
        void Dispose<TComponent>( TComponent component ) where TComponent : IComponent;
    }

    internal class ComponentDisposer : IComponentDisposer
    {
        #region Members

        private readonly IJSRuntime jsRuntime;

        private bool disposePossible;

        private readonly IList<object> disposables;

        private const string PROPERTY_DISPOSABLES = "Disposables";

        private static Func<object, IList<object>> disposablesGetter;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="jsRuntime">Instance of a JavaScript runtime to which calls may be dispatched.</param>
        /// <param name="serviceProvider">Service provider used to retrieve registered components.</param>
        public ComponentDisposer( IJSRuntime jsRuntime, IServiceProvider serviceProvider )
        {
            this.jsRuntime = jsRuntime;

            ServiceProvider = serviceProvider;
            disposables = LoadServiceProviderDisposableList();
        }

        #endregion

        #region Methods

        public void Dispose<TComponent>( TComponent component ) where TComponent : IComponent
        {
            if ( !disposePossible )
                return;

            if ( component is BaseAfterRenderComponent afterRenderComponent && ( afterRenderComponent.Disposed || afterRenderComponent.AsyncDisposed ) )
            {
                if ( disposables.Contains( component ) )
                    disposables.Remove( component );
            }
        }

        /// <summary>
        /// Loads the ServiceProvider's list of disposables
        /// </summary>
        /// <returns>List of object references the ServiceProvider uses to track disposables</returns>
        private IList<object> LoadServiceProviderDisposableList()
        {
            if ( IsWebAssembly )
            {
                disposePossible = false;

                return new List<object>();
            }

            if ( disposablesGetter is null )
                disposablesGetter = ExpressionCompiler.CreatePropertyGetter<IList<object>>( ServiceProvider, PROPERTY_DISPOSABLES );

            var disposables = disposablesGetter( ServiceProvider );

            disposePossible = !disposables.IsNullOrEmpty();

            return disposables;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the current app is running in webassembly mode.
        /// </summary>
        protected bool IsWebAssembly => jsRuntime is IJSInProcessRuntime;

        /// <summary>
        /// Gets the reference to the <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        #endregion
    }
}
