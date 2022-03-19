#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A modal provider to be set at the root of your app, providing a programatic way to invoke modals with custom content by using ModalService.
    /// </summary>
    public partial class ModalProvider
    {
        #region Members

        private Modal modalRef;
        private RenderFragment childContent;
        private bool useModalDefinition = true; //TOOD : Choose a better name
        private string title = "example";

        #endregion

        #region Constructors



        #endregion

        #region Methods

        protected override Task OnInitializedAsync()
        {
            ModalService.SetModalProvider( this );
            return base.OnInitializedAsync();
        }
        public Task Show( RenderFragment childContent )
        {
            this.childContent = childContent;
            return modalRef.Show();
        }

        public Task Hide()
            => modalRef.Hide();

        #endregion

        #region Properties

        [Inject] public IModalService ModalService { get; set; }

        #endregion
    }

    /// <summary>
    /// Together with ModalProvider, handles the instantion of modals with custom content.
    /// </summary>
    public interface IModalService
    {

        /// <summary>
        /// Required Modal Provider that manages the instantiation of custom content.
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
        /// Hides currently opened modal.
        /// </summary>
        /// <returns></returns>
        public Task Hide();
    }

    public class ModalService : IModalService
    {
        //TODO: 
        //Pass TComponent Parameters
        //Pass Modal Parameters?

        //inhericdoc
        public ModalProvider ModalProvider { get; private set; }

        //inhericdoc
        public void SetModalProvider( ModalProvider modalProvider )
        {
            ModalProvider = modalProvider;
        }

        //inhericdoc
        public Task Show<TComponent>()
            => Show( typeof( TComponent ) );

        //inhericdoc
        public Task Show( Type componentType )
        {
            var childContent = new RenderFragment( __builder =>
            {
                __builder.OpenComponent( 0, componentType );
                __builder.CloseComponent();
            } );

            return ModalProvider.Show( childContent );
        }

        //inhericdoc
        public Task Hide()
            => ModalProvider.Hide();
    }
}
