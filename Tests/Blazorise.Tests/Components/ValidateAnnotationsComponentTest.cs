using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Xunit;


namespace Blazorise.Tests.Components;

public class ValidateAnnotationsComponentTest : TestContext
{
    private const string NameRequired = "The Name field is required.";
    private const string PasswordLength = "The field Password must be a string with a minimum length of 5 and a maximum length of 8.";
    private const string PasswordWithDisplayLength = "The field DisplayName:Some.Custom.Name must be a string with a minimum length of 5 and a maximum length of 8.";
    private const string ErrorOverride = "error override message";

    public ValidateAnnotationsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseButton()
            .AddBlazoriseTextEdit();
    }

    [Fact]
    public async Task CanAutoValidateName_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-name-initially-blank input" );

        Assert.Contains( "is-invalid", edit.ClassList );

        // test 1
        await edit.InputAsync( "a" );
        Assert.Contains( "is-valid", edit.ClassList );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.ClassList );

        var feedback = comp.Find( "#auto-validate-name-initially-blank .invalid-feedback" );

        Assert.NotNull( feedback );
        Assert.Contains( NameRequired, feedback.TextContent );
    }

    [Fact]
    public async Task CanAutoValidateName_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-name-initially-populated input" );

        Assert.Contains( "is-valid", edit.ClassList );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.ClassList );

        var feedback = comp.Find( "#auto-validate-name-initially-populated .invalid-feedback" );

        Assert.NotNull( feedback );
        Assert.Contains( NameRequired, feedback.TextContent );

        // test 2
        await edit.InputAsync( "b" );
        Assert.Contains( "is-valid", edit.ClassList );
    }

    [Fact]
    public async Task CanAutoValidatePassword_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-password-initially-blank input" );

        Assert.Contains( "is-invalid", edit.ClassList );

        // test 1
        await edit.InputAsync( "1234" );
        Assert.Contains( "is-invalid", edit.ClassList );

        var feedback = comp.Find( "#auto-validate-password-initially-blank .invalid-feedback" );
        Assert.NotNull( feedback );
        Assert.Contains( PasswordLength, feedback.TextContent );

        // test 2
        await edit.InputAsync( "12345" );
        Assert.Contains( "is-valid", edit.ClassList );

        // test 3
        await edit.InputAsync( "12345678" );
        Assert.Contains( "is-valid", edit.ClassList );

        // test 4
        await edit.InputAsync( "123456789" );
        Assert.Contains( "is-invalid", edit.ClassList );

        // test 5
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.ClassList );
    }

    [Fact]
    public async Task CanAutoValidatePassword_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-password-initially-populated input" );

        Assert.Contains( "is-valid", edit.ClassList );

        // test 1
        await edit.InputAsync( "1234" );
        Assert.Contains( "is-invalid", edit.ClassList );

        var feedback = comp.Find( "#auto-validate-password-initially-populated .invalid-feedback" );
        Assert.NotNull( feedback );
        Assert.Contains( PasswordLength, feedback.TextContent );

        // test 2
        await edit.InputAsync( "12345" );
        Assert.Contains( "is-valid", edit.ClassList );

        // test 3
        await edit.InputAsync( "12345678" );
        Assert.Contains( "is-valid", edit.ClassList );

        // test 4
        await edit.InputAsync( "123456789" );
        Assert.Contains( "is-invalid", edit.ClassList );

        // test 5
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.ClassList );
    }

    [Fact]
    public async Task CanAutoValidatePasswordWithDisplay_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-password-with-display-initially-blank input" );

        Assert.Contains( "is-invalid", edit.ClassList );

        // test 1
        await edit.InputAsync( "1234" );
        Assert.Contains( "is-invalid", edit.ClassList );

        var feedback = comp.Find( "#auto-validate-password-with-display-initially-blank .invalid-feedback" );
        Assert.NotNull( feedback );
        Assert.Contains( PasswordWithDisplayLength, feedback.TextContent );
    }

    [Fact]
    public async Task CanManuallyValidateName_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#manually-validate-name-initially-blank input" );
        var btn = comp.Nodes.QuerySelector( "#manually-validate-name-initially-blank button" );

        Assert.DoesNotContain( "is-invalid", edit.ClassList );
        Assert.DoesNotContain( "is-valid", edit.ClassList );

        // test 1
        await btn.ClickAsync();
        Assert.Contains( "is-invalid", edit.ClassList );

        // test 2
        await edit.InputAsync( "a" );
        Assert.Contains( "is-invalid", edit.ClassList );
        Assert.DoesNotContain( "is-valid", edit.ClassList );
        await btn.ClickAsync();
        Assert.Contains( "is-valid", edit.ClassList );

        // test 3
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-valid", edit.ClassList );
        await btn.ClickAsync();
        Assert.Contains( "is-invalid", edit.ClassList );
    }

    [Fact]
    public async Task CanManuallyValidateName_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#manually-validate-name-initially-populated input" );
        var btn = comp.Find( "#manually-validate-name-initially-populated button" );

        Assert.DoesNotContain( "is-invalid", edit.ClassList );
        Assert.DoesNotContain( "is-valid", edit.ClassList );

        // test 1
        await btn.ClickAsync();
        Assert.Contains( "is-valid", edit.ClassList );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.DoesNotContain( "is-invalid", edit.ClassList );
        Assert.Contains( "is-valid", edit.ClassList );
        await btn.ClickAsync();
        Assert.Contains( "is-invalid", edit.ClassList );

        // test 3
        await edit.InputAsync( "a" );
        Assert.Contains( "is-invalid", edit.ClassList );
        await btn.ClickAsync();
        Assert.Contains( "is-valid", edit.ClassList );
    }

    [Fact]
    public async Task CanAutoValidateName_InitiallyPopulated_ErrorOverride()
    {
        // setup
        var comp = RenderComponent<ValidateAnnotationsComponent>();
        var edit = comp.Find( "#auto-validate-name-initially-populated-error-override input" );

        Assert.Contains( "is-valid", edit.ClassList );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.ClassList );

        var feedback = comp.Find( "#auto-validate-name-initially-populated-error-override .invalid-feedback" );

        Assert.NotNull( feedback );
        Assert.Contains( ErrorOverride, feedback.TextContent );

        // test 2
        await edit.InputAsync( "b" );
        Assert.Contains( "is-valid", edit.ClassList );
    }
}