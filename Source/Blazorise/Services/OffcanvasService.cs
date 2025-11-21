using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Components.OffcanvasProvider;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazorise;
public  class OffcanvasService : IOffcanvasService
{
    /// <inheritdoc/>
    public OffcanvasProvider OffcanvasProvider { get; private set; }

    /// <inheritdoc/>
    public void SetOffcanvasProvider( OffcanvasProvider offcanvasProvider )
        => OffcanvasProvider = offcanvasProvider;

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>() where TComponent : notnull, IComponent
        => Show( typeof( TComponent ) );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>( string title ) where TComponent : notnull, IComponent
        => Show( title, typeof( TComponent ) );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>( string title, OffcanvasInstanceOptions OffcanvasProviderOptions ) where TComponent : notnull, IComponent
        => Show<TComponent>( title, null, OffcanvasProviderOptions );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent
        => Show<TComponent>( string.Empty, parameters, null );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters, OffcanvasInstanceOptions OffcanvasInstanceOptions ) where TComponent : notnull, IComponent
        => Show<TComponent>( string.Empty, parameters, OffcanvasInstanceOptions );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>( string title, Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent
        => Show<TComponent>( title, parameters, null );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show<TComponent>( string title, Action<OffcanvasProviderParameterBuilder<TComponent>> parameters, OffcanvasInstanceOptions OffcanvasInstanceOptions ) where TComponent : notnull, IComponent
    {
        RenderFragment childContent = BuildParameterfulContent<TComponent>( parameters );
        return Show( title, childContent, OffcanvasInstanceOptions );
    }

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show( Type componentType )
        => Show( string.Empty, componentType );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show( string title, Type componentType )
        => Show( title, componentType, null, null );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show( RenderFragment content )
        => Show( string.Empty, content );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show( string title, RenderFragment content )
        => Show( title, content, null );

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show( string title, Type componentType, Dictionary<string, object> componentParameters = null, OffcanvasInstanceOptions OffcanvasInstanceOptions = null )
    {
        RenderFragment childContent = BuildParameterfulContent( componentType, componentParameters );
        return Show( title, childContent, OffcanvasInstanceOptions );
    }

    /// <inheritdoc/>
    public Task<OffcanvasInstance> Show( string title, RenderFragment childContent, OffcanvasInstanceOptions OffcanvasInstanceOptions = null )
    {
        return OffcanvasProvider.Show( title, childContent, OffcanvasInstanceOptions ?? new() );
    }

    /// <inheritdoc/>
    public Task Show( OffcanvasInstance OffcanvasInstance )
        => OffcanvasProvider.Show( OffcanvasInstance );

    /// <inheritdoc/>
    public Task Hide()
        => OffcanvasProvider.Hide();

    /// <inheritdoc/>
    public Task Hide( OffcanvasInstance OffcanvasInstance )
        => OffcanvasProvider.Hide( OffcanvasInstance );

    /// <inheritdoc/>
    public IEnumerable<OffcanvasInstance> GetInstances()
        => OffcanvasProvider.GetInstances();

    /// <inheritdoc/>
    public Task Reset()
        => OffcanvasProvider.Reset();

    /// <inheritdoc/>
    public Task Remove( OffcanvasInstance OffcanvasInstance )
        => OffcanvasProvider.Remove( OffcanvasInstance );

    private static RenderFragment BuildParameterfulContent( Type componentType, Dictionary<string, object> componentParameters )
    {
        return new RenderFragment( __builder =>
        {
            __builder.OpenComponent( componentType );

            if ( componentParameters is not null )
            {
                __builder.Attributes( componentParameters, 1 );
            }

            __builder.CloseComponent();
        } );
    }

    private static RenderFragment BuildParameterfulContent<TComponent>( Action<OffcanvasProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent
    {
        return new RenderFragment( __builder =>
        {
            __builder.OpenComponent<TComponent>();

            if ( parameters is not null )
            {
                OffcanvasProviderParameterBuilder<TComponent> parameterBuilder = new();
                parameters( parameterBuilder );

                if ( parameterBuilder.Parameters is not null )
                {
                    __builder.Attributes( parameterBuilder.Parameters, 1 );
                }
            }

            __builder.CloseComponent();
        } );
    }
}
