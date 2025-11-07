using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class NumericInputComponentTest : TestContext
{
    public NumericInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseNumericInput();
    }

    [Fact]
    public async Task CanChangeUndefinedIntegerUsingEvent()
    {
        // setup
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#int-event-initially-undefined" );
        var numeric = comp.Find( "#int-undefined-numeric" );
        var result = comp.Find( "#int-event-initially-undefined-result" );

        Assert.Equal( "0", result.InnerHtml );

        // test 1
        await numeric.InputAsync( "100" );
        Assert.Equal( "100", result.InnerHtml );

        // test 2
        await numeric.InputAsync( "10" );
        Assert.Equal( "10", result.InnerHtml );
    }

    [Fact]
    public async Task CanChangeNullableIntegerUsingEvent()
    {
        // setup
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#nullable-int-event-initially-null" );
        var numeric = comp.Find( "#int-nullable-numeric" );
        var result = comp.Find( "#nullable-int-event-initially-null-result" );

        Assert.Equal( string.Empty, result.InnerHtml );

        // test 1
        await numeric.InputAsync( "100" );
        Assert.Equal( "100", result.InnerHtml );

        // test 2
        await numeric.InputAsync( "10" );
        Assert.Equal( "10", result.InnerHtml );
    }

    [Fact]
    public async Task CanChangeUndefinedDecimalUsingEvent()
    {
        // setup
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#decimal-event-initially-undefined" );
        var numeric = comp.Find( "#decimal-undefined-numeric" );
        var result = comp.Find( "#decimal-event-initially-undefined-result" );

        Assert.Equal( "0", result.InnerHtml );

        // test 1
        await numeric.InputAsync( "200" );
        Assert.Equal( "200", result.InnerHtml );

        // test 2
        await numeric.InputAsync( "1" );
        Assert.Equal( "1", result.InnerHtml );
    }

    [Fact]
    public async Task CanChangeNullableDecimalUsingEvent()
    {
        // setup
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#nullable-decimal-event-initially-null" );
        var numeric = comp.Find( "#decimal-nullable-numeric" );
        var result = comp.Find( "#nullable-decimal-event-initially-null-result" );

        Assert.Equal( string.Empty, result.InnerHtml );

        // test 1
        await numeric.InputAsync( "1000" );
        Assert.Equal( "1000", result.InnerHtml );

        // test 2
        await numeric.InputAsync( "10" );
        Assert.Equal( "10", result.InnerHtml );
    }

    /* todo: figure out how to send an Up key in bUnit.
    [Fact]
    public void CanChangeValueWithStepDefault()
    {
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#step-change-default" );
        var numeric = comp.Find( "#step-default-numeric" );
        var result = comp.Find( "#step-change-default" );

        Assert.Equal( "1", result.InnerHtml );
        
        numeric.KeyPress( "Keys.Up" );
        numeric.KeyPress( "Keys.Up" );
        Assert.Equal( "3", result.InnerHtml );

        numeric.KeyPress( "Keys.Down" );
        Assert.Equal( "2", result.InnerHtml );
    }

    [Fact]
    public void CanChangeValueWithStepBy2()
    {
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#step-change-by-2" );
        var numeric = comp.Find( "#step-2-numeric" );
        var result = comp.Find( "#step-change-by-2-result" );

        Assert.Equal( "2", result.InnerHtml );

        numeric.KeyPress( "Keys.Up" );
        numeric.KeyPress( "Keys.Up" );
        Assert.Equal( "6", result.InnerHtml );

        numeric.KeyPress( "Keys.Down" );
        Assert.Equal( "4", result.InnerHtml );
    }
    */

    [Fact]
    public async Task CanTypeNumberWithDotDecimalSeparator()
    {
        // setup
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#decimal-separator-with-dot" );
        var numeric = comp.Find( "#dot-decimal-numeric" );
        var result = comp.Find( "#decimal-separator-with-dot-result" );

        Assert.Equal( "42.5", result.InnerHtml );

        // test 1
        await numeric.InputAsync( "42.56" );
        Assert.Equal( "42.56", result.InnerHtml );

        // test 2
        await numeric.InputAsync( "42.3" );
        Assert.Equal( "42.3", result.InnerHtml );
    }

    [Fact]
    public async Task CanTypeNumberWithCommaDecimalSeparator()
    {
        // setup
        var comp = RenderComponent<NumericInputComponent>();
        var paragraph = comp.Find( "#decimal-separator-with-comma" );
        var numeric = comp.Find( "#comma-decimal-numeric" );
        var result = comp.Find( "#decimal-separator-with-comma-result" );

        Assert.Equal( "42,5", result.InnerHtml );

        // test 1
        await numeric.InputAsync( "42,56" );
        Assert.Equal( "42,56", result.InnerHtml );

        // test 2
        await numeric.InputAsync( "42,3" );
        Assert.Equal( "42,3", result.InnerHtml );
    }
}