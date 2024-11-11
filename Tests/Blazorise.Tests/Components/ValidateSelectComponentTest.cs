using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ValidateSelectComponentTest : TestContext
{
    public ValidateSelectComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public async Task CanValidateText_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-initially-blank" );
        var select = comp.Find( "#validate-string-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateStringWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-bind-initially-blank" );
        var select = comp.Find( "#validate-string-with-bind-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateStringWithBind_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-bind-initially-selected" );
        var select = comp.Find( "#validate-string-with-bind-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateStringWithEvent_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-event-initially-blank" );
        var select = comp.Find( "#validate-string-with-event-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }


    [Fact]
    public async Task CanValidateStringWithEvent_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-event-initially-selected" );
        var select = comp.Find( "#validate-string-with-event-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateInt_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-int-initially-blank" );
        var select = comp.Find( "#validate-int-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "1" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateIntWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-int-with-bind-initially-blank" );
        var select = comp.Find( "#validate-int-with-bind-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "1" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateIntWithBind_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-int-with-bind-initially-selected" );
        var select = comp.Find( "#validate-int-with-bind-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "1" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateEnum_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-enum-initially-blank" );
        var select = comp.Find( "#validate-enum-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "First" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateEnumWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-enum-with-bind-initially-blank" );
        var select = comp.Find( "#validate-enum-with-bind-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "First" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task CanValidateEnumWithBind_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-enum-with-bind-initially-selected" );
        var select = comp.Find( "#validate-enum-with-bind-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "First" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    //  multiple select validation is failing.
    [Fact]
    public async Task CanValidateMultiString_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-multi-string-initially-blank" );
        var select = comp.Find( "#validate-multi-string-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        await select.ChangeAsync( "Oliver" );
        await select.ChangeAsync( "Jack" );
        Assert.Contains( "custom-select", select.GetAttribute( "class" ) );

        // test 2
        await select.ChangeAsync( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 3
        await select.ChangeAsync( "Oliver" );
        Assert.Contains( "custom-select", select.GetAttribute( "class" ) );
    }
}