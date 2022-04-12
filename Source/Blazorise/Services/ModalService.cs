#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Blazorise.Extensions;
#endregion

namespace Blazorise
{
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
        public Task<ModalInstance> Show<TComponent>()
            => Show( typeof( TComponent ) );


        /// <inheritdoc/>
        public Task<ModalInstance> Show<TComponent>( string title )
            => Show( title, typeof( TComponent ) );

        /// <inheritdoc/>
        public Task<ModalInstance> Show<TComponent>( string title, ModalProviderOptions modalProviderOptions )
            => Show<TComponent>( title, null, modalProviderOptions );

        /// <inheritdoc/>
        public Task<ModalInstance> Show<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters )
            => Show<TComponent>( string.Empty, parameters, null );


        /// <inheritdoc/>
        public Task<ModalInstance> Show<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters, ModalProviderOptions modalProviderOptions )
            => Show<TComponent>( string.Empty, parameters, modalProviderOptions );

        /// <inheritdoc/>
        public Task<ModalInstance> Show<TComponent>( string title, Action<ModalProviderParameterBuilder<TComponent>> parameters )
            => Show<TComponent>( title, parameters, null );

        /// <inheritdoc/>
        public Task<ModalInstance> Show<TComponent>( string title, Action<ModalProviderParameterBuilder<TComponent>> parameters, ModalProviderOptions modalProviderOptions )
        {
            ModalProviderParameterBuilder<TComponent> builder = new();
            if ( parameters is not null )
                parameters( builder );
            return Show( title, typeof( TComponent ), builder.Parameters, modalProviderOptions );
        }

        /// <inheritdoc/>
        public Task<ModalInstance> Show( Type componentType )
            => Show( string.Empty, componentType );


        /// <inheritdoc/>
        public Task<ModalInstance> Show( string title, Type componentType )
            => Show( title, componentType, null, null );

        /// <inheritdoc/>
        public Task<ModalInstance> Show( string title, Type componentType, Dictionary<string, object> componentParameters = null, ModalProviderOptions modalProviderOptions = null )
        {
            var childContent = new RenderFragment( __builder =>
            {
                var i = 0;
                __builder.OpenComponent( componentType );
                if ( componentParameters is not null )
                {
                    __builder.Attributes( componentParameters );
                }
                __builder.CloseComponent();
            } );

            return ModalProvider.Show( title, childContent, modalProviderOptions );
        }

        /// <inheritdoc/>
        public Task Hide()
            => ModalProvider.Hide();

        /// <inheritdoc/>
        public Task Hide( ModalInstance modalInstance )
            => ModalProvider.Hide( modalInstance );
    }
}
