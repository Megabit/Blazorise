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
    public void CanValidateText_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-initially-blank" );
        var select = comp.Find( "#validate-string-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateStringWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-bind-initially-blank" );
        var select = comp.Find( "#validate-string-with-bind-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateStringWithBind_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-bind-initially-selected" );
        var select = comp.Find( "#validate-string-with-bind-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateStringWithEvent_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-event-initially-blank" );
        var select = comp.Find( "#validate-string-with-event-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }


    [Fact]
    public void CanValidateStringWithEvent_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-string-with-event-initially-selected" );
        var select = comp.Find( "#validate-string-with-event-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "Harry" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateInt_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-int-initially-blank" );
        var select = comp.Find( "#validate-int-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "1" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateIntWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-int-with-bind-initially-blank" );
        var select = comp.Find( "#validate-int-with-bind-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "1" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateIntWithBind_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-int-with-bind-initially-selected" );
        var select = comp.Find( "#validate-int-with-bind-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "1" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateEnum_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-enum-initially-blank" );
        var select = comp.Find( "#validate-enum-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "First" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateEnumWithBind_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-enum-with-bind-initially-blank" );
        var select = comp.Find( "#validate-enum-with-bind-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "First" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    [Fact]
    public void CanValidateEnumWithBind_InitiallySelected()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-enum-with-bind-initially-selected" );
        var select = comp.Find( "#validate-enum-with-bind-initially-selected select" );

        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "First" );
        Assert.Contains( "is-valid", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );
    }

    //  multiple select validation is failing.
    [Fact]
    public void CanValidateMultiString_InitiallyBlank()
    {
        // setup
        var comp = RenderComponent<ValidateSelectComponent>();
        var paragraph = comp.Find( "#validate-multi-string-initially-blank" );
        var select = comp.Find( "#validate-multi-string-initially-blank select" );

        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 1
        select.Change( "Oliver" );
        select.Change( "Jack" );
        Assert.Contains( "custom-select", select.GetAttribute( "class" ) );

        // test 2
        select.Change( "" );
        Assert.Contains( "is-invalid", select.GetAttribute( "class" ) );

        // test 3
        select.Change( "Oliver" );
        Assert.Contains( "custom-select", select.GetAttribute( "class" ) );
    }
}