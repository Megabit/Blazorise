using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.FluentUI2;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

// FileInput wraps the native input, so utility classes (eg. Display, Margin) must target the
// wrapper element and not the inner input - otherwise eg. Display.None does not hide the control (issue #6522).
// The same behaviour is verified across every provider that renders a wrapper around the input.
public abstract class FileInputUtilityComponentTestBase : BunitContext
{
    protected FileInputUtilityComponentTestBase()
    {
        Services.AddBlazoriseTests();
        RegisterProviders( Services );
        Services.AddEmptyIconProvider().AddTestData();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected abstract void RegisterProviders( IServiceCollection services );

    [Fact]
    public void DisplayUtility_IsApplied_ToWrapper_NotInput()
    {
        // the display class is resolved from the active provider so the test stays provider-agnostic
        var classProvider = Services.GetRequiredService<IClassProvider>();
        var displayNoneClass = Display.None.Class( classProvider );

        var comp = Render<FileInput>( parameters => parameters
            .Add( p => p.Display, Display.None ) );

        var input = comp.Find( "input[type=file]" );

        // the inner input must NOT receive the utility class
        Assert.DoesNotContain( displayNoneClass, input.GetAttribute( "class" ) ?? string.Empty );

        // the wrapper element must receive it (so the whole control is hidden)
        Assert.NotEmpty( comp.FindAll( $".{displayNoneClass}" ) );
    }

    [Fact]
    public void NoUtility_DoesNotEmit_UtilityClasses()
    {
        var classProvider = Services.GetRequiredService<IClassProvider>();
        var displayNoneClass = Display.None.Class( classProvider );

        var comp = Render<FileInput>();

        // without utilities nothing is hidden anywhere
        Assert.Empty( comp.FindAll( $".{displayNoneClass}" ) );
    }
}

public class FileInputBootstrapUtilityComponentTest : FileInputUtilityComponentTestBase
{
    protected override void RegisterProviders( IServiceCollection services )
        => services.AddBootstrapProviders();
}

public class FileInputBootstrap5UtilityComponentTest : FileInputUtilityComponentTestBase
{
    protected override void RegisterProviders( IServiceCollection services )
        => services.AddBootstrap5Providers();
}

public class FileInputFluentUI2UtilityComponentTest : FileInputUtilityComponentTestBase
{
    protected override void RegisterProviders( IServiceCollection services )
        => services.AddFluentUI2Providers();
}
