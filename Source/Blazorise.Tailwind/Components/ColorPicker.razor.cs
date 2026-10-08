#region Using directives
using Blazorise.Utilities;
#endregion

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
            builder.Append( "group/color-picker relative" );

            if ( PreviewContent is not null )
            {
                builder.Append( ClassProvider.TextInput( false ) );
                builder.Append( ClassProvider.TextInputSize( ThemeSize ) );
                builder.Append( "data-[custom-preview=true]:inline-flex data-[custom-preview=true]:h-auto items-center" );
                builder.Append( "[&>input]:absolute [&>input]:inset-0 [&>input]:w-full [&>input]:h-full [&>input]:m-0! [&>input]:opacity-0 [&>input]:cursor-pointer [&>input:disabled]:cursor-not-allowed" );
                builder.Append( "focus-within:outline-solid focus-within:outline-2 focus-within:outline-offset-2 focus-within:outline-primary-600 has-[>input:disabled]:opacity-[.65]" );
                builder.Append( UtilityClassBuilder.Class );
                builder.Append( Class );
            }

            AppendWrapperUtilities( builder );
        }

        private void BuildWrapperStyles( StyleBuilder builder )
        {
            if ( PreviewContent is not null )
            {
                builder.Append( UtilityStyleBuilder.Styles );
                builder.Append( Style );
            }

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

        protected override string ColorPreviewElementSelector => null;

        protected string WrapperClassNames => wrapperClassBuilder.Class;

        protected string WrapperStyleNames => wrapperStyleBuilder.Styles;

        #endregion
    }
}