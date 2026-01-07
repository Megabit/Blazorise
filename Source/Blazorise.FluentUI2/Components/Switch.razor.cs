#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class Switch<TValue>
{
    #region Constructors

    public Switch()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses, builder => builder.Append( Classes?.Wrapper ) );
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
        builder.Append( "fui-Switch" );

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Switch-error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Switch-success" );
        }

        if ( ThemeSize != Blazorise.Size.Default )
        {
            builder.Append( $"fui-Switch-{ClassProvider.ToSize( ThemeSize )}" );
        }

        if ( Disabled )
        {
            builder.Append( "fui-Switch-disabled" );
        }
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string AddonClassNames
    {
        get
        {
            if ( string.IsNullOrEmpty( Classes?.Wrapper ) )
                return "fui-Input__content";

            return $"fui-Input__content {Classes.Wrapper}";
        }
    }

    #endregion
}