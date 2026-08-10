#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class FileInput
{
    #region Constructors

    public FileInput()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses );
        AddonClassBuilder = new ClassBuilder( BuildAddonClasses );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        InputClassBuilder.Dirty();
        AddonClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private void BuildInputClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Input" );

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Input-error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Warning )
        {
            builder.Append( "fui-Input-warning" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Input-success" );
        }

        if ( ThemeSize != Blazorise.Size.Default )
        {
            builder.Append( $"fui-Input-{ClassProvider.ToSize( ThemeSize )}" );
        }

        if ( Disabled )
        {
            builder.Append( "disabled" );
        }

        builder.Append( WrapperClassNames );
    }

    private void BuildAddonClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Input__content" );
        builder.Append( WrapperClassNames );
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected ClassBuilder AddonClassBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string AddonClassNames => AddonClassBuilder.Class;

    #endregion
}