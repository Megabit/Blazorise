#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class MemoEdit
{
    #region Constructors

    public MemoEdit()
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
        builder.Append( "fui-Textarea" );

        if ( Plaintext )
        {
            builder.Append( "fui-Textarea-plaintext" );
        }

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Textarea__error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Textarea__success" );
        }

        if ( ThemeSize != Blazorise.Size.Default )
        {
            builder.Append( $"fui-Textarea-{ClassProvider.ToSize( ThemeSize )}" );
        }

        if ( Disabled )
        {
            builder.Append( "disabled" );
        }
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string AddonClassNames => "fui-Input__content";

    #endregion
}
