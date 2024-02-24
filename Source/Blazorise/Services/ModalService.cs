#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Together with ModalProvider, handles the instantiation of modals with custom content.
/// </summary>
public class ModalService : IModalService
{
    /// <inheritdoc/>
    public ModalProvider ModalProvider { get; private set; }

    /// <inheritdoc/>
    public void SetModalProvider( ModalProvider modalProvider )
        => ModalProvider = modalProvider;

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>() where TComponent : notnull, IComponent
        => Show( typeof( TComponent ) );

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>( string title ) where TComponent : notnull, IComponent
        => Show( title, typeof( TComponent ) );

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>( string title, ModalInstanceOptions modalProviderOptions ) where TComponent : notnull, IComponent
        => Show<TComponent>( title, null, modalProviderOptions );

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent
        => Show<TComponent>( string.Empty, parameters, null );

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters, ModalInstanceOptions modalInstanceOptions ) where TComponent : notnull, IComponent
        => Show<TComponent>( string.Empty, parameters, modalInstanceOptions );

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>( string title, Action<ModalProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent
        => Show<TComponent>( title, parameters, null );

    /// <inheritdoc/>
    public Task<ModalInstance> Show<TComponent>( string title, Action<ModalProviderParameterBuilder<TComponent>> parameters, ModalInstanceOptions modalInstanceOptions ) where TComponent : notnull, IComponent
    {
        RenderFragment childContent = BuildParameterfulContent<TComponent>( parameters );
        return Show( title, childContent, modalInstanceOptions );
    }

    /// <inheritdoc/>
    public Task<ModalInstance> Show( Type componentType )
        => Show( string.Empty, componentType );

    /// <inheritdoc/>
    public Task<ModalInstance> Show( string title, Type componentType )
        => Show( title, componentType, null, null );

    /// <inheritdoc/>
    public Task<ModalInstance> Show( RenderFragment content )
        => Show( string.Empty, content );

    /// <inheritdoc/>
    public Task<ModalInstance> Show( string title, RenderFragment content )
        => Show( title, content, null );

    /// <inheritdoc/>
    public Task<ModalInstance> Show( string title, Type componentType, Dictionary<string, object> componentParameters = null, ModalInstanceOptions modalInstanceOptions = null )
    {
        RenderFragment childContent = BuildParameterfulContent( componentType, componentParameters );
        return Show( title, childContent, modalInstanceOptions );
    }

    /// <inheritdoc/>
    public Task<ModalInstance> Show( string title, RenderFragment childContent, ModalInstanceOptions modalInstanceOptions = null )
    {
        return ModalProvider.Show( title, childContent, modalInstanceOptions );
    }

    /// <inheritdoc/>
    public Task Show( ModalInstance modalInstance )
        => ModalProvider.Show( modalInstance );

    /// <inheritdoc/>
    public Task Hide()
        => ModalProvider.Hide();

    /// <inheritdoc/>
    public Task Hide( ModalInstance modalInstance )
        => ModalProvider.Hide( modalInstance );

    /// <inheritdoc/>
    public IEnumerable<ModalInstance> GetInstances()
        => ModalProvider.GetInstances();

    /// <inheritdoc/>
    public Task Reset()
        => ModalProvider.Reset();

    /// <inheritdoc/>
    public Task Remove( ModalInstance modalInstance )
        => ModalProvider.Remove( modalInstance );

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

    private static RenderFragment BuildParameterfulContent<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters ) where TComponent : notnull, IComponent
    {
        return new RenderFragment( __builder =>
        {
            __builder.OpenComponent<TComponent>();

            if ( parameters is not null )
            {
                ModalProviderParameterBuilder<TComponent> parameterBuilder = new();
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