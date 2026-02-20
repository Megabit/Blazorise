#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class FieldLabel : Blazorise.FieldLabel
{
    #region Members

    private ClassBuilder containerClassBuilder;
    private StyleBuilder containerStyleBuilder;

    #endregion

    #region Constructors

    public FieldLabel()
    {
        containerClassBuilder = new( BuildContainerClasses, builder => builder.Append( Classes?.Container ) );
        containerStyleBuilder = new( BuildContainerStyles, builder => builder.Append( Styles?.Container ) );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        containerClassBuilder.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        containerStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    private void BuildContainerClasses( ClassBuilder builder )
    {
        builder.Append( "ant-col" );
        builder.Append( "ant-form-item-label" );
        AppendWrapperUtilities( builder );

        if ( ColumnSize != null )
        {
            builder.Append( ColumnSize.Class( false, ClassProvider ) );
        }
    }

    private void BuildContainerStyles( StyleBuilder builder )
    {
        AppendWrapperUtilities( builder );
    }

    #endregion

    #region Properties

    protected override bool PreventColumnSize => true; // disable column-size on the label, as we're going to add it on container

    protected string ContainerClassNames => containerClassBuilder.Class;

    protected string ContainerStyleNames => containerStyleBuilder.Styles;

    #endregion
}