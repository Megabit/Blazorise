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
    }

    #endregion

    #region Methods

    protected virtual void BuildWrapperClasses( ClassBuilder builder )
    {
        builder.Append( "col" );
    }

    protected internal override void DirtyClasses()
    {
        WrapperClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    protected ClassBuilder WrapperClassBuilder { get; private set; }

    protected string WrapperClassNames => WrapperClassBuilder.Class;

    #endregion
}