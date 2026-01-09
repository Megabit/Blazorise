#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class Toast
{
    #region Constructors

    public Toast()
    {
        WrapperClassBuilder = new ClassBuilder( BuildWrapperClasses, builder => builder.Append( Classes?.Wrapper ) );
        WrapperStyleBuilder = new StyleBuilder( BuildWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        WrapperClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        WrapperStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    private void BuildWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "fui-ToastContainer" );
        builder.Append( WrapperFade( Animated && State.Showing, Animated && State.Hiding ) );
        builder.Append( WrapperVisible( IsVisible ) );
        AppendWrapperUtilities( builder );
    }

    private void BuildWrapperStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ToastAnimationDuration( Animated, AnimationDuration ) );
        AppendWrapperUtilities( builder );
    }

    private static string WrapperVisible( bool visible ) => visible
        ? "fui-ToastContainer-show"
        : "fui-ToastContainer-hide";

    private static string WrapperFade( bool showing, bool hiding ) => showing
        ? "fui-ToastContainer-showing"
        : hiding
            ? "fui-ToastContainer-hiding"
            : null;

    #endregion

    #region Properties

    protected ClassBuilder WrapperClassBuilder { get; private set; }

    protected StyleBuilder WrapperStyleBuilder { get; private set; }

    protected string WrapperClassNames => WrapperClassBuilder.Class;

    protected string WrapperStyleNames => WrapperStyleBuilder.Styles;

    #endregion
}