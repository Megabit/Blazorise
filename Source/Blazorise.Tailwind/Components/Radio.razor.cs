#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class Radio<TValue>
{
    private ClassBuilder buttonLabelClassBuilder;

    private StyleBuilder buttonLabelStyleBuilder;

    private StyleBuilder buttonInputStyleBuilder;

    protected override void OnInitialized()
    {
        buttonLabelClassBuilder = new( BuildButtonLabelClasses );
        buttonLabelStyleBuilder = new( BuildButtonLabelStyles );
        buttonInputStyleBuilder = new( BuildButtonInputStyles );

        base.OnInitialized();
    }

    protected internal override void DirtyClasses()
    {
        buttonLabelClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        buttonLabelStyleBuilder?.Dirty();
        buttonInputStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    private void BuildButtonLabelClasses( ClassBuilder builder )
    {
        builder.Append( "relative focus:ring-4 font-medium focus:outline-none" );
        builder.Append( ClassProvider.ButtonSize( ThemeSize, false ) );

        if ( ParentRadioGroup?.Orientation == Orientation.Horizontal )
        {
            builder.Append( "rounded-none first:rounded-l-lg last:rounded-r-lg" );
        }
        else
        {
            builder.Append( "rounded-none first:rounded-t-lg last:rounded-b-lg w-full" );
        }

        if ( ButtonColor?.IsCssValue == true )
        {
            builder.Append( ClassProvider.ButtonColor( ButtonColor, false ) );
            builder.Append( ClassProvider.ButtonActive( false, IsActive ) );
            builder.Append( "has-[:focus-visible]:ring-4 has-[:focus-visible]:ring-[color-mix(in_srgb,var(--tw-button-bg)_40%,transparent)]" );
        }
        else if ( IsActive )
        {
            builder.Append( "text-white bg-secondary-700 hover:bg-secondary-800 focus:ring-secondary-300 dark:bg-secondary-600 dark:hover:bg-secondary-700 dark:focus:ring-secondary-800" );
        }
        else
        {
            builder.Append( "text-white bg-secondary-500 hover:bg-secondary-600 focus:ring-secondary-100 dark:bg-secondary-400 dark:hover:bg-secondary-500 dark:focus:ring-secondary-600" );
        }

        builder.Append( "cursor-not-allowed opacity-60", IsDisabled );
    }

    private void BuildButtonLabelStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ButtonColor( ButtonColor ) );
    }

    private void BuildButtonInputStyles( StyleBuilder builder )
    {
        builder.Append( "clip: rect(0,0,0,0)" );
        builder.Append( StyleNames );
    }

    private string ButtonLabelClassNames => buttonLabelClassBuilder.Class;

    private string ButtonLabelStyleNames => buttonLabelStyleBuilder.Styles;

    private string ButtonInputStyleNames => buttonInputStyleBuilder.Styles;
}