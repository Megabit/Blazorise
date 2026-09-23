#region Using directives
using Blazorise.Utilities;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class Switch<TValue>
{
    private ClassBuilder backgroundClassBuilder;

    private StyleBuilder backgroundStyleBuilder;

    protected override void OnInitialized()
    {
        backgroundClassBuilder = new( BuildBackgroundClasses );
        backgroundStyleBuilder = new( BuildBackgroundStyles );

        base.OnInitialized();
    }

    protected internal override void DirtyClasses()
    {
        backgroundClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    protected internal override void DirtyStyles()
    {
        backgroundStyleBuilder?.Dirty();

        base.DirtyStyles();
    }

    private void BuildBackgroundClasses( ClassBuilder builder )
    {
        builder.Append( SwitchSize( ThemeSize ) );

        if ( Color?.IsCssValue == true )
        {
            builder.Append( "bg-gray-200 dark:bg-gray-700 rounded-full peer-focus-visible:ring-4 peer-focus-visible:ring-[color-mix(in_srgb,var(--tw-switch-bg)_40%,transparent)] peer-checked:bg-[var(--tw-switch-bg)] peer-disabled:opacity-50 peer-disabled:cursor-not-allowed peer-checked:after:translate-x-full after:content-[''] after:absolute after:bg-white after:border-gray-300 after:border after:rounded-full after:transition-all peer-checked:after:border-transparent supports-[color:contrast-color(red)]:peer-checked:after:bg-[contrast-color(var(--tw-switch-bg))]" );
        }
        else
        {
            string colorName = string.IsNullOrEmpty( Color?.Name ) ? "primary" : Color.Name;

            builder.Append( $"bg-gray-200 rounded-full peer peer-focus:ring-4 peer-focus:ring-{colorName}-300 dark:peer-focus:ring-{colorName}-800 dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:bg-white after:border-gray-300 after:border after:rounded-full after:transition-all dark:border-gray-600 peer-checked:bg-{colorName}-600" );
        }
    }

    private void BuildBackgroundStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.SwitchColor( Color ) );
    }

    private static string SwitchSize( Size size )
    {
        return size switch
        {
            Blazorise.Size.ExtraSmall => "w-8 h-4 after:h-3 after:w-3 after:top-1 after:left-[3px]",
            Blazorise.Size.Small => "w-9 h-5 after:h-4 after:w-4 after:top-[2px] after:left-[2px]",
            Blazorise.Size.Medium => "w-12 h-7 after:h-5 after:w-5 after:top-1 after:left-[3px]",
            Blazorise.Size.Large => "w-14 h-8 after:h-6 after:w-6 after:top-1 after:left-[3px]",
            Blazorise.Size.ExtraLarge => "w-16 h-10 after:h-7 after:w-7 after:top-1.5 after:left-[3px]",
            _ => "w-11 h-6 after:h-5 after:w-5 after:top-0.5 after:left-[2px]"
        };
    }

    private string BackgroundClassNames => backgroundClassBuilder.Class;

    private string BackgroundStyleNames => backgroundStyleBuilder.Styles;
}