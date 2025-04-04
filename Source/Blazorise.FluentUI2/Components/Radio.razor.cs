#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.FluentUI2.Components;

public partial class Radio<TValue>
{
    #region Constructors

    public Radio()
    {
        InputClassBuilder = new ClassBuilder( BuildInputClasses );
        LabelButonClassBuilder = new ClassBuilder( BuildLabelButonClasses );
    }

    #endregion

    #region Methods

    protected internal override void DirtyClasses()
    {
        if ( AsButton )
        {
            LabelButonClassBuilder.Dirty();
        }
        else
        {
            InputClassBuilder.Dirty();
        }

        base.DirtyClasses();
    }

    private void BuildInputClasses( ClassBuilder builder )
    {
        builder.Append( "fui-Radio" );

        if ( ParentValidation?.Status == ValidationStatus.Error )
        {
            builder.Append( "fui-Radio-error" );
        }
        else if ( ParentValidation?.Status == ValidationStatus.Success )
        {
            builder.Append( "fui-Radio-success" );
        }

        if ( Disabled )
        {
            builder.Append( "disabled" );
        }
    }

    private void BuildLabelButonClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Button( false ) );
        builder.Append( ClassProvider.ButtonColor( ButtonColor, false ) );
        builder.Append( ClassProvider.ButtonActive( false, IsActive ) );
        builder.Append( ClassProvider.ButtonDisabled( false, Disabled ) );
    }

    #endregion

    #region Properties

    protected ClassBuilder InputClassBuilder { get; private set; }

    protected ClassBuilder LabelButonClassBuilder { get; private set; }

    protected string InputClassNames => InputClassBuilder.Class;

    protected string LabelButonClassNames => LabelButonClassBuilder.Class;

    protected string AddonClassNames => "fui-Input__content";

    #endregion
}
