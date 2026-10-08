using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Blazorise.Tests.Components;

public class NumericInputComponentTest : BunitContext
{
    public NumericInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseNumericInput();
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public async Task NativeNumericInputPreservesEditingValueUntilCompletedOrReplaced( bool immediate )
    {
        // setup
        decimal value = 15.5m;
        IRenderedComponent<NumericInput<decimal>> comp = Render<NumericInput<decimal>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue )
            .Add( x => x.Immediate, immediate )
            .Add( x => x.Debounce, false ) );

        // test
        await comp.Find( "input" ).TriggerEventAsync( immediate ? "oninput" : "onchange", new ChangeEventArgs { Value = string.Empty } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate existing default-value behavior without writing it back into the input
        Assert.Equal( 0m, value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).TriggerEventAsync( immediate ? "oninput" : "onchange", new ChangeEventArgs { Value = "0012." } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate the exact editing string survives the parent echo
        Assert.Equal( 12m, value );
        Assert.Equal( "0012.", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).TriggerEventAsync( immediate ? "oninput" : "onchange", new ChangeEventArgs { Value = "-12.5" } );

        // validate
        Assert.Equal( -12.5m, value );
        Assert.Equal( "-12.5", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        comp.Render( parameters => parameters.Add( x => x.Value, 43.5m ) );

        // validate
        Assert.Equal( "43.5", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).TriggerEventAsync( immediate ? "oninput" : "onchange", new ChangeEventArgs { Value = "0043.5" } );
        Assert.Equal( "0043.5", comp.Find( "input" ).GetAttribute( "value" ) );
        comp.Render( parameters => parameters.Add( x => x.Culture, "de-DE" ) );

        // validate existing culture formatting after replacing the editing string
        Assert.Equal( "43,5", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public async Task NullableNativeNumericInputPreservesEmptyEditingValueAcrossRenders( bool immediate )
    {
        // setup
        decimal? value = 15.5m;
        IRenderedComponent<NumericInput<decimal?>> comp = Render<NumericInput<decimal?>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue )
            .Add( x => x.Immediate, immediate )
            .Add( x => x.Debounce, false ) );

        // test
        await comp.Find( "input" ).TriggerEventAsync( immediate ? "oninput" : "onchange", new ChangeEventArgs { Value = string.Empty } );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate
        Assert.Null( value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).TriggerEventAsync( immediate ? "oninput" : "onchange", new ChangeEventArgs { Value = "0012.5" } );

        // validate
        Assert.Equal( 12.5m, value );
        Assert.Equal( "0012.5", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        comp.Render( parameters => parameters.Add( x => x.Value, (decimal?)null ) );

        // validate
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task DebouncedNativeNumericInputPreservesEditingValueBeforeAndAfterCommit()
    {
        // setup
        decimal value = 15.5m;
        IRenderedComponent<NumericInput<decimal>> comp = Render<NumericInput<decimal>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue )
            .Add( x => x.Immediate, true )
            .Add( x => x.Debounce, true )
            .Add( x => x.DebounceInterval, 60000 ) );

        // test
        await comp.Find( "input" ).InputAsync( string.Empty );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );

        // validate the model remains unchanged until the debounce is flushed
        Assert.Equal( 15.5m, value );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).KeyPressAsync( new KeyboardEventArgs { Key = "Enter" } );

        // validate
        comp.WaitForAssertion( () => Assert.Equal( 0m, value ) );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );
        Assert.Equal( string.Empty, comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).InputAsync( "0012.5" );

        // validate
        Assert.Equal( 0m, value );
        Assert.Equal( "0012.5", comp.Find( "input" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "input" ).KeyPressAsync( new KeyboardEventArgs { Key = "Enter" } );

        // validate
        comp.WaitForAssertion( () => Assert.Equal( 12.5m, value ) );
        comp.Render( parameters => parameters.Add( x => x.Value, value ) );
        Assert.Equal( "0012.5", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Theory]
    [InlineData( "-5", 0 )]
    [InlineData( "25", 20 )]
    public async Task NativeNumericInputDisplaysClampedValueOnBlur( string inputValue, int expectedValue )
    {
        // setup
        decimal value = 15m;
        IRenderedComponent<NumericInput<decimal>> comp = Render<NumericInput<decimal>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.ValueChanged, changedValue => value = changedValue )
            .Add( x => x.Min, 0m )
            .Add( x => x.Max, 20m )
            .Add( x => x.Immediate, true )
            .Add( x => x.Debounce, false ) );

        // test
        await comp.Find( "input" ).InputAsync( inputValue );
        Assert.Equal( inputValue, comp.Find( "input" ).GetAttribute( "value" ) );
        await comp.Find( "input" ).BlurAsync();

        // validate
        Assert.Equal( (decimal)expectedValue, value );
        Assert.Equal( expectedValue.ToString(), comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task CanChangeUndefinedIntegerUsingEvent()
    {
        // setup
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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
        var comp = Render<NumericInputComponent>();
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