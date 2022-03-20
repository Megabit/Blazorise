#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// A modal provider to be set at the root of your app, providing a programmatic way to invoke modals with custom content by using ModalService.
    /// </summary>
    public partial class ModalProvider : ComponentBase
    {
        #region Members

        private Modal modalRef;
        private RenderFragment childContent;
        private bool useModalStructure = true;
        private string title;

        #endregion

        #region Constructors



        #endregion

        #region Methods

        ///inheritdoc
        protected override Task OnInitializedAsync()
        {
            ModalService.SetModalProvider( this );
            return base.OnInitializedAsync();
        }


        internal Task Show( RenderFragment childContent )
        {
            this.childContent = childContent;
            return modalRef.Show();
        }


        internal Task Hide()
            => modalRef.Hide();

        #endregion

        #region Properties

        ///inheritdoc
        [Inject] public IModalService ModalService { get; set; }

        #endregion
    }

    /// <summary>
    /// Sets the options for Modal Provider
    /// </summary>
    public class ModalProviderOptions
    {
        /// <summary>
        /// Uses the modal standard structure, by setting this to true you are only in charge of providing the custom content.
        /// Defaults to true.
        /// </summary>
        public bool UseModalStructure { get; set; } = true;

        /// <summary>
        /// Gets or Sets the modal's title.
        /// </summary>
        public string Title { get; set; }

    }

    /// <summary>
    /// Provide the parameters to be passed onto your custom content to be rendered under the modal.
    /// </summary>
    public class ModalComponentParameter
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Constructs a new ModalComponentParameter.
        /// </summary>
        /// <param name="name">Parameter Name</param>
        /// <param name="value">Parameter Value</param>
        public ModalComponentParameter( string name, object value )
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// Provides a builder with helper methods to help you build your component parameters.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public class ModalComponentParameterBuilder<TComponent>
    {
        private List<ModalComponentParameter> parameters;
        public IEnumerable<ModalComponentParameter> Parameters => parameters;

        public void Add<TValue>( Expression<Func<TComponent, TValue>> selector, TValue value )
        {
            var name = ( ( selector.Body ) as MemberExpression ).Member.Name;
            AddNewModalComponentParameter( name, value );
        }

        private void AddNewModalComponentParameter( string name, object value )
        {
            parameters ??= new List<ModalComponentParameter>();
            parameters.Add( new ModalComponentParameter( name, value ) );
        }

    }

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
        public Task Show<TComponent>( Action<ModalComponentParameterBuilder<TComponent>> parameters );

        /// <summary>
        /// Shows a Modal where the content is of componentType.
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="modalComponentParameters"></param>
        /// <returns></returns>
        public Task Show( Type componentType, IEnumerable<ModalComponentParameter> modalComponentParameters );

        /// <summary>
        /// Hides currently opened modal.
        /// </summary>
        /// <returns></returns>
        public Task Hide();
    }

    /// <summary>
    /// Together with ModalProvider, handles the instantiation of modals with custom content.
    /// </summary>
    public class ModalService : IModalService
    {
        //TODO: 
        //Pass TComponent Parameters
        //Pass Modal Parameters?

        ///inheritdoc
        public ModalProvider ModalProvider { get; private set; }

        ///inheritdoc
        public void SetModalProvider( ModalProvider modalProvider )
        {
            ModalProvider = modalProvider;
        }

        ///inheritdoc
        public Task Show<TComponent>( Action<ModalComponentParameterBuilder<TComponent>> parameters )
        {
            ModalComponentParameterBuilder<TComponent> builder = new();
            parameters( builder );
            return Show( typeof( TComponent ), builder.Parameters );
        }

        ///inheritdoc
        public Task Show<TComponent>()
        {
            //componentParameters.Select( x => x() )?.ToList();
            return Show( typeof( TComponent ), null );
        }

        ///inheritdoc
        public Task Show( Type componentType )
        {
            //componentParameters.Select( x => x() )?.ToList();
            return Show( componentType, null );
        }

        ///inheritdoc
        public Task Show( Type componentType, IEnumerable<ModalComponentParameter> modalComponentParameters )
        {
            var childContent = new RenderFragment( __builder =>
            {
                var i = 0;
                __builder.OpenComponent( i++, componentType );
                if ( modalComponentParameters is not null )
                {
                    foreach ( var parameter in modalComponentParameters )
                    {
                        __builder.AddAttribute( i++, parameter.Name, parameter.Value );
                    }
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
