#region Using directives
using Blazorise.Extensions;
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class Step : Blazorise.Step
{
    #region Constructors

    public Step()
    {
        ContainerClassBuilder = new( BuildContainerClasses, builder => builder.Append( Classes?.Container ) );
        ContainerStyleBuilder = new( BuildContainerStyles, builder => builder.Append( Styles?.Container ) );
    }

    #endregion

    #region Methods

    protected override void BuildMarkerClasses( ClassBuilder builder )
    {
        var color = Completed && Color.IsNullOrDefault() ? Color.Primary : Color;

        builder.Append( ClassProvider.StepItemMarker() );
        builder.Append( ClassProvider.StepItemMarkerColor( color, Active ) );
    }

    protected virtual void BuildContainerClasses( ClassBuilder builder )
    {
        builder.Append( "tw-step-container flex min-w-0 flex-col items-start" );
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

    protected ClassBuilder ContainerClassBuilder { get; }

    protected StyleBuilder ContainerStyleBuilder { get; }

    protected string ContainerClassNames => ContainerClassBuilder.Class;

    protected string ContainerStyleNames => ContainerStyleBuilder.Styles;

    #endregion
}