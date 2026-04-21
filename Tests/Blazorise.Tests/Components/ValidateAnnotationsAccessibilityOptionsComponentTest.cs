using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class ValidateAnnotationsAccessibilityOptionsComponentTest : BunitContext
{
    private const string NameRequired = "The Name field is required.";

    public ValidateAnnotationsAccessibilityOptionsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.UseValidationOnBlur = true;
        } ) );
        JSInterop
            .AddBlazoriseButton()
            .AddBlazoriseTextInput();
    }

    [Fact]
    public async Task CanAutoValidateName_OnBlur_WhenValidationOnBlurIsEnabled()
    {
        // setup
        var comp = Render<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-name-on-blur input" );

        Assert.DoesNotContain( "is-invalid", edit.ClassList );
        Assert.DoesNotContain( "is-valid", edit.ClassList );

        // test
        await edit.FocusAsync( new() );
        await edit.BlurAsync( new() );

        comp.WaitForAssertion( () =>
        {
            Assert.Contains( "is-invalid", edit.ClassList );
        } );

        var feedback = comp.Find( "#auto-validate-name-on-blur .invalid-feedback" );

        Assert.NotNull( feedback );
        Assert.Contains( NameRequired, feedback.TextContent );
    }
}