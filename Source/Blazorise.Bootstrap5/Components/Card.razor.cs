#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap5.Components;

public partial class Card : Blazorise.Card
{
    #region Constructors

    public Card()
    {
        WrapperClassBuilder = new( BuildWrapperClasses, builder => builder.Append( Classes?.Wrapper ) );
        WrapperStyleBuilder = new( BuildWrapperStyles, builder => builder.Append( Styles?.Wrapper ) );
    }

    #endregion

    #region Methods

    protected virtual void BuildWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "col" );
        AppendWrapperUtilities( builder );
    }

    protected virtual void BuildWrapperStyles( StyleBuilder builder )
    {
        AppendWrapperUtilities( builder );
    }

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

    #endregion

    #region Properties

    protected ClassBuilder WrapperClassBuilder { get; private set; }

    protected StyleBuilder WrapperStyleBuilder { get; private set; }

    protected string WrapperClassNames => WrapperClassBuilder.Class;

    protected string WrapperStyleNames => WrapperStyleBuilder.Styles;

    #endregion
}