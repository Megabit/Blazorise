#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap.Components;

public partial class Step : Blazorise.Step
{
    #region Constructors

    public Step()
        : base()
    {
        ContainerClassBuilder = new( BuildContainerClasses, builder => builder.Append( Classes?.Container ) );
        ContainerStyleBuilder = new( BuildContainerStyles, builder => builder.Append( Styles?.Container ) );
    }

    #endregion

    #region Methods

    protected virtual void BuildContainerClasses( ClassBuilder builder )
    {
        builder.Append( "step-container" );
        AppendWrapperUtilities( builder );
    }

    protected virtual void BuildContainerStyles( StyleBuilder builder )
    {
        AppendWrapperUtilities( builder );
    }

    protected internal override void DirtyClasses()
    {
        ContainerClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        ContainerStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    #endregion

    #region Properties

    protected ClassBuilder ContainerClassBuilder { get; private set; }

    protected StyleBuilder ContainerStyleBuilder { get; private set; }

    protected string ContainerClassNames
        => ContainerClassBuilder.Class;

    protected string ContainerStyleNames
        => ContainerStyleBuilder.Styles;

    #endregion
}