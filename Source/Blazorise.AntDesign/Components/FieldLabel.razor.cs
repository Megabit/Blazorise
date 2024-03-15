#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class FieldLabel : Blazorise.FieldLabel
{
    #region Members

    private ClassBuilder containerClassBuilder;

    #endregion

    #region Constructors

    public FieldLabel()
    {
        containerClassBuilder = new( BuildContainerClasses );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        containerClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private void BuildContainerClasses( ClassBuilder builder )
    {
        builder.Append( "ant-col" );
        builder.Append( "ant-form-item-label" );

        if ( ColumnSize != null )
        {
            builder.Append( ColumnSize.Class( false, ClassProvider ) );
        }
    }

    #endregion

    #region Properties

    protected override bool PreventColumnSize => true; // disable column-size on the label, as we're going to add it on container

    protected string ContainerClassNames => containerClassBuilder.Class;

    #endregion
}