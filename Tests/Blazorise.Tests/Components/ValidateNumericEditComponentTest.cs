using System.Diagnostics;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Extensions;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;


public class ValidateNumericEditComponentTest : TestContext
{
    public ValidateNumericEditComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddNumericEdit( this.JSInterop );
    }

    [Fact]
    public async Task CanValidateNumeric_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateNumericEditComponent>();
        var paragraph = comp.Find( "#validate-numeric-initially-blank" );
        var edit = comp.Find( "#validate-numeric-initially-blank input" );

        Debug.WriteLine( comp.Markup );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "1" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateNumeric_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateNumericEditComponent>();
        var paragraph = comp.Find( "#validate-numeric-initially-populated" );
        var edit = comp.Find( "#validate-numeric-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "2" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateNumericWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateNumericEditComponent>();
        var paragraph = comp.Find( "#validate-numeric-with-bind-initially-blank" );
        var edit = comp.Find( "#validate-numeric-with-bind-initially-blank input" );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "1" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateNumericWithBind_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateNumericEditComponent>();
        var paragraph = comp.Find( "#validate-numeric-with-bind-initially-populated" );
        var edit = comp.Find( "#validate-numeric-with-bind-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "2" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateNumericWithEvent_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateNumericEditComponent>();
        var paragraph = comp.Find( "#validate-numeric-with-event-initially-blank" );
        var edit = comp.Find( "#validate-numeric-with-event-initially-blank input" );

        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( "1" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateNumericWithEvent_InitiallyPopulated()
    {
        // setup
        var comp = RenderComponent<ValidateNumericEditComponent>();
        var paragraph = comp.Find( "#validate-numeric-with-event-initially-populated" );
        var edit = comp.Find( "#validate-numeric-with-event-initially-populated input" );

        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );

        // test 1
        await edit.InputAsync( string.Empty );
        Assert.Contains( "is-invalid", edit.GetAttribute( "class" ) );

        // test 2
        await edit.InputAsync( "2" );
        Assert.Contains( "is-valid", edit.GetAttribute( "class" ) );
    }
}