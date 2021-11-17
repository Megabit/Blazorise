#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap
{
    public partial class ModalContent : Blazorise.ModalContent
    {
        #region Members

        #endregion

        #region Constructors

        public ModalContent()
        {
            DialogClassBuilder = new( BuildDialogClasses );
        }

        #endregion

        #region Methods

        protected internal override void DirtyClasses()
        {
            DialogClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildDialogClasses( ClassBuilder builder )
        {
            builder.Append( "modal-dialog" );
            builder.Append( $"modal-{ClassProvider.ToModalSize( Size )}", Size != ModalSize.Default );
            builder.Append( ClassProvider.ModalContentCentered( Centered ) );

            if ( Fullscreen )
                builder.Append( "modal-fullscreen" );

            if ( Centered )
                builder.Append( "modal-dialog-centered" );

            if ( Scrollable )
                builder.Append( "modal-dialog-scrollable" );
        }

        #endregion

        #region Properties

        protected ClassBuilder DialogClassBuilder { get; private set; }

        /// <summary>
        /// Gets dialog container class-names.
        /// </summary>
        protected string DialogClassNames => DialogClassBuilder.Class;

        #endregion
    }
}
