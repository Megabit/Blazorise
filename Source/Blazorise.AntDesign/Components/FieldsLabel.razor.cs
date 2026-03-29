#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class FieldsLabel : Blazorise.FieldsLabel
{
    #region Members

    private ClassBuilder containerClassBuilder;
    private StyleBuilder containerStyleBuilder;

    #endregion

    #region Constructors

    public FieldsLabel()
    {
        containerClassBuilder = new( BuildContainerClasses );
        containerStyleBuilder = new( BuildContainerStyles );
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

    protected override bool PreventColumnSize => true;

    protected string ContainerClassNames => containerClassBuilder.Class;

    protected string ContainerStyleNames => containerStyleBuilder.Styles;

    #endregion
}