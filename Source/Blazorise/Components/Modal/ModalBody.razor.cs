#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ModalBody : BaseComponent
    {
        #region Members

        private int? maxHeight;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ModalBody() );

            base.BuildClasses( builder );
        }

        protected override void BuildStyles( StyleBuilder builder )
        {
            if ( MaxHeight != null )
                builder.Append( StyleProvider.ModalBodyMaxHeight( MaxHeight ?? 0 ) );

            base.BuildStyles( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the maximum height of the modal body (in viewport size unit).
        /// </summary>
        [Parameter]
        public int? MaxHeight
        {
            get => maxHeight;
            set
            {
                maxHeight = value;

                DirtyStyles();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
