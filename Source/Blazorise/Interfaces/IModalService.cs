#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Together with ModalProvider, handles the instantiation of modals with custom content.
    /// </summary>
    public interface IModalService
    {

        /// <summary>
        /// Required Modal Provider that manages the instantiation of modals with custom content.
        /// </summary>
        public ModalProvider ModalProvider { get; }

        /// <summary>
        /// Sets the required Modal Provider.
        /// </summary>
        /// <param name="modalProvider"></param>
        public void SetModalProvider( ModalProvider modalProvider );

        /// <summary>
        /// Shows a Modal where the content is TComponent.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        public Task Show<TComponent>();

        /// <summary>
        /// Shows a Modal where the content is of componentType.
        /// </summary>
        /// <param name="componentType"></param>
        public Task Show( Type componentType );

        /// <summary>
        /// Shows a Modal where the content is TComponent.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task Show<TComponent>( Action<ModalProviderParameterBuilder<TComponent>> parameters );

        /// <summary>
        /// Shows a Modal where the content is of componentType.
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="componentParameters"></param>
        /// <returns></returns>
        public Task Show( Type componentType, Dictionary<string, object> componentParameters );

        /// <summary>
        /// Hides currently opened modal.
        /// </summary>
        /// <returns></returns>
        public Task Hide();
    }
}
