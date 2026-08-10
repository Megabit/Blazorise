using Blazorise.Utilities;

namespace Blazorise.Tailwind.Components
{
    public partial class ColorPicker
    {
        #region Members

        private readonly ClassBuilder wrapperClassBuilder;

        private readonly StyleBuilder wrapperStyleBuilder;

        #endregion

        #region Constructors

        public ColorPicker()
        {
            wrapperClassBuilder = new( BuildWrapperClasses, builder => builder.Append( Classes?.Wrapper ) );
            wrapperStyleBuilder = new( BuildWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
        }

        #endregion

        #region Methods

        private void BuildWrapperClasses( ClassBuilder builder )
        {
            builder.Append( "relative" );
            AppendWrapperUtilities( builder );
        }

        private void BuildWrapperStyles( StyleBuilder builder )
        {
            AppendWrapperUtilities( builder );
        }

        protected internal override void DirtyClasses()
        {
            wrapperClassBuilder.Dirty();

            base.DirtyClasses();
        }

        protected internal override void DirtyStyles()
        {
            wrapperStyleBuilder.Dirty();

            base.DirtyStyles();
        }

        #endregion

        #region Properties

        protected string WrapperClassNames => wrapperClassBuilder.Class;

        protected string WrapperStyleNames => wrapperStyleBuilder.Styles;

        #endregion
    }
}