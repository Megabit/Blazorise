#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

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

    private bool active;

    private bool disposePossible;

    private bool retried;

    private IList<object> disposables;

    private const string FIELD_DISPOSABLES = "_disposables";

    private const string DEFAULT_DOTNET_SERVICEPROVIDER = "ServiceProviderEngineScope";

    private static Func<object, IList<object>> disposablesGetter;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="serviceProvider">Service provider used to retrieve registered components.</param>
    public ComponentDisposer( IServiceProvider serviceProvider )
    {
        ServiceProvider = serviceProvider;

        active = ServiceProvider.GetType().Name.Equals( DEFAULT_DOTNET_SERVICEPROVIDER, StringComparison.InvariantCultureIgnoreCase );

        if ( active )
            disposables = LoadServiceProviderDisposableList();
    }

    #endregion

    #region Methods

    public void Dispose<TComponent>( TComponent component ) where TComponent : IComponent
    {
        if ( !active )
            return;

        if ( !disposePossible && !retried )
        {
            disposables = LoadServiceProviderDisposableList();
            retried = true;
        }

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
        if ( disposablesGetter is null )
            disposablesGetter = ExpressionCompiler.CreateFieldGetter<IList<object>>( ServiceProvider, FIELD_DISPOSABLES );

        var disposables = disposablesGetter( ServiceProvider );

        disposePossible = !disposables.IsNullOrEmpty();

        return disposables;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the reference to the <see cref="IServiceProvider"/>.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    #endregion
}