using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Components.OffcanvasProvider;
using Microsoft.AspNetCore.Components;

namespace Blazorise;

public interface IOffcanvasService
{
    /// <inheritdoc/>
    OffcanvasProvider OffcanvasProvider { get; }

    /// <inheritdoc/>
    void SetOffcanvasProvider( OffcanvasProvider offcanvasProvider );

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>() where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>( string title ) where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>( string title, OffcanvasInstanceOptions? offcanvasInstanceOptions ) where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters, OffcanvasInstanceOptions offcanvasInstanceOptions ) where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>( string title, Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show<TComponent>( string title, Action<OffcanvasProviderParameterBuilder<TComponent>> parameters, OffcanvasInstanceOptions offcanvasInstanceOptions ) where TComponent : notnull, IComponent;

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show( Type componentType );

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show( string title, Type componentType );

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show( RenderFragment content );

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show( string title, RenderFragment content );

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show( string title, Type componentType, Dictionary<string, object> componentParameters = null, OffcanvasInstanceOptions offcanvasInstanceOptions = null );

    /// <inheritdoc/>
    Task<OffcanvasInstance> Show( string title, RenderFragment childContent, OffcanvasInstanceOptions offcanvasInstanceOptions = null );

    /// <inheritdoc/>
    Task Show( OffcanvasInstance offcanvasInstance );

    /// <inheritdoc/>
    Task Hide();

    /// <inheritdoc/>
    Task Hide( OffcanvasInstance offcanvasInstance );

    /// <inheritdoc/>
    IEnumerable<OffcanvasInstance> GetInstances();

    /// <inheritdoc/>
    Task Reset();

    /// <inheritdoc/>
    Task Remove( OffcanvasInstance offcanvasInstance );
}
