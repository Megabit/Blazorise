#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Tailwind.Components;

public partial class Progress
{
    private ClassBuilder hasProgressBarClassBuilder;

    private ClassBuilder innerClassBuilder;

    protected override void OnInitialized()
    {
        hasProgressBarClassBuilder = new( BuildHasProgressBarClasses );
        innerClassBuilder = new( BuildInnerClasses, builder => builder.Append( Classes?.Bar ) );

        base.OnInitialized();
    }

    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.IsParameterChanged( ChildContent ) )
        {
            DirtyClasses();
        }

        return base.SetParametersAsync( parameters );
    }

    protected internal override void DirtyClasses()
    {
        hasProgressBarClassBuilder?.Dirty();
        innerClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    private void BuildHasProgressBarClasses( ClassBuilder builder )
    {
        builder.Append( "w-full bg-gray-200 rounded-full dark:bg-gray-700 mb-3 flex overflow-hidden" );

        if ( ChildContent is null )
        {
            builder.Append( ClassProvider.ProgressSize( ThemeSize ) );
        }
    }

    private void BuildInnerClasses( ClassBuilder builder )
    {
        builder.Append( "text-xs font-medium text-center p-0.5 leading-none rounded-full" );

        if ( Color.IsNotNullOrDefault() )
        {
            builder.Append( ClassProvider.ProgressBarColor( Color ) );
        }

        if ( ChildContent is null )
        {
            builder.Append( ClassProvider.ProgressSize( ThemeSize ) );
        }

        builder.Append( "bg-striped", Striped );
        builder.Append( "bg-striped-animated", Animated );
        builder.Append( "progress-bar-indeterminate", Indeterminate );
    }

    private string HasProgressBarClassNames => hasProgressBarClassBuilder.Class;

    private string InnerClassNames => innerClassBuilder.Class;
}