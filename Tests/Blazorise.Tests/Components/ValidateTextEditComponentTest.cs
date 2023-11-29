using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ValidateTextEditComponentTest : TestContext
{
    public ValidateTextEditComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop.AddBlazoriseTextEdit();
    }

    [Fact]
    public async Task CanValidateText_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-initially-blank" );
        var edit = comp.Find( "#validate-text-initially-blank input" );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "a" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateText_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-initially-populated" );
        var edit = comp.Find( "#validate-text-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "b" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateTextWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-with-bind-initially-blank" );
        var edit = comp.Find( "#validate-text-with-bind-initially-blank input" );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "a" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateTextWithBind_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-with-bind-initially-populated" );
        var edit = comp.Find( "#validate-text-with-bind-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "b" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateTextWithEvent_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-with-event-initially-blank" );
        var edit = comp.Find( "#validate-text-with-event-initially-blank input" );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "a" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateTextWithEvent_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-with-event-initially-populated" );
        var edit = comp.Find( "#validate-text-with-event-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "b" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidatePattern_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-using-pattern-initially-blank" );
        var edit = comp.Find( "#validate-text-using-pattern-initially-blank input" );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "a" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "1" );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 3
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidatePattern_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateTextEditComponent>();
        var paragraph = comp.Find( "#validate-text-using-pattern-initially-populated" );
        var edit = comp.Find( "#validate-text-using-pattern-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "b" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 3
        await edit.InputAsync( "2" );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }
}