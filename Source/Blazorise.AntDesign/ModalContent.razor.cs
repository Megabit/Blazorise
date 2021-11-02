#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ParentModal.NotifyCloseActivatorIdInitialized( WrapperElementId ??= IdGenerator.Generate );
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DisposeResources();
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                DisposeResources();
            }

            return base.DisposeAsync( disposing );
        }

        private void DisposeResources()
        {
            ParentModal.NotifyCloseActivatorIdRemoved( WrapperElementId );
        }

        protected internal override void DirtyClasses()
        {
            DialogClassBuilder.Dirty();

            base.DirtyClasses();
        }

        private void BuildDialogClasses( ClassBuilder builder )
        {
            builder.Append( "ant-modal" );
            builder.Append( $"ant-modal-{ClassProvider.ToModalSize( Size )}" );
        }

        #endregion

        #region Properties

        protected string WrapperElementId { get; set; }

        protected ClassBuilder DialogClassBuilder { get; private set; }

        /// <summary>
        /// Gets dialog container class-names.
        /// </summary>
        protected string DialogClassNames => DialogClassBuilder.Class;

        #endregion
    }
}
