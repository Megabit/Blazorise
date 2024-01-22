#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class ColorEdit
{
    #region Constructors

    public ColorEdit()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        InputClassBuilder.Dirty();

        base.DirtyClasses();
    }

    private void BuildInputClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Input" );

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Input-error" );
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
    }

    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"--fui-ColorInput--value: {CurrentValueAsString}" );

        base.BuildStyles( builder );
    }

    protected override Task OnInternalValueChanged( string value )
    {
        DirtyStyles();

        return base.OnInternalValueChanged( value );
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string AddonClassNames => "fui-Input__content";

    #endregion
}
