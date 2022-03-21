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
        ///inheritdoc
        public ModalProvider ModalProvider { get; private set; }

        ///inheritdoc
        public void SetModalProvider( ModalProvider modalProvider )
        {
            ModalProvider = modalProvider;
        }

        ///inheritdoc
        public Task Show<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters )
        {
            ModalProviderParameterBuilder<TComponent> builder = new();
            parameters( builder );
            return Show( typeof( TComponent ), builder.Parameters );
        }

        ///inheritdoc
        public Task Show<TComponent>()
        {
            return Show( typeof( TComponent ), null );
        }

        ///inheritdoc
        public Task Show( Type componentType )
        {
            return Show( componentType, null );
        }

        ///inheritdoc
        public Task Show( Type componentType, Dictionary<string, object> componentParameters )
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

            return ModalProvider.Show( childContent );
        }

        ///inheritdoc
        public Task Hide()
            => ModalProvider.Hide();
    }
}
