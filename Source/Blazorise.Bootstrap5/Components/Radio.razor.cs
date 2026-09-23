#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Bootstrap5.Components;

public partial class Radio<TValue>
{
    private ClassBuilder labelButtonClassBuilder;

    private StyleBuilder labelButtonStyleBuilder;

    protected override void OnInitialized()
    {
        labelButtonClassBuilder = new( BuildLabelButtonClasses );
        labelButtonStyleBuilder = new( BuildLabelButtonStyles );

        base.OnInitialized();
    }

    protected internal override void DirtyClasses()
    {
        labelButtonClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        labelButtonStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    private void BuildLabelButtonClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Button( false ) );
        builder.Append( ClassProvider.ButtonColor( ButtonColor, false ) );
        builder.Append( ClassProvider.ButtonActive( false, IsActive ) );
        builder.Append( ClassProvider.ButtonDisabled( false, IsDisabled ) );
    }

    private void BuildLabelButtonStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ButtonColor( ButtonColor ) );
    }

    private string LabelButtonClassNames => labelButtonClassBuilder.Class;

    private string LabelButtonStyleNames => labelButtonStyleBuilder.Styles;
}