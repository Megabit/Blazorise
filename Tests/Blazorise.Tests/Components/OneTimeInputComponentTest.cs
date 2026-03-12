using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Components;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class OneTimeInputComponentTest : TestContext
{
    public OneTimeInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseTextInput();
    }

    [Fact]
    public void RendersDefaultGroupingForSixDigits()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add( x => x.Digits, 6 ) );

        var inputs = comp.FindAll( "input" );
        var groups = comp.FindAll( ".b-one-time-input-group" );
        var separators = comp.FindAll( ".b-one-time-input-separator" );

        Assert.Equal( 6, inputs.Count );
        Assert.Equal( 2, groups.Count );
        Assert.Equal( 1, separators.Count );
        Assert.Equal( 3, groups[0].QuerySelectorAll( "input" ).Length );
        Assert.Equal( 3, groups[1].QuerySelectorAll( "input" ).Length );
    }

    [Fact]
    public void RendersCustomGrouping()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add( x => x.Digits, 5 )
            .Add( x => x.Group, "2,3" ) );

        var groups = comp.FindAll( ".b-one-time-input-group" );
        var separators = comp.FindAll( ".b-one-time-input-separator" );

        Assert.Equal( 2, groups.Count );
        Assert.Equal( 1, separators.Count );
        Assert.Equal( 2, groups[0].QuerySelectorAll( "input" ).Length );
        Assert.Equal( 3, groups[1].QuerySelectorAll( "input" ).Length );
    }

    [Fact]
    public async Task MovesFocusToNextEmptySlotWhenPasteDoesNotFillRemainingSlots()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add( x => x.Digits, 4 ) );

        var inputs = comp.FindAll( "input" );

        await inputs[0].InputAsync( "12" );

        inputs = comp.FindAll( "input" );

        Assert.Equal( "12", comp.Instance.Value );
        Assert.Equal( "1", inputs[0].GetAttribute( "value" ) );
        Assert.Equal( "2", inputs[1].GetAttribute( "value" ) );
        Assert.True( string.IsNullOrEmpty( inputs[2].GetAttribute( "value" ) ) );

        comp.WaitForAssertion( () =>
        {
            var invocation = JSInterop.VerifyInvoke( "focus" );

            Assert.Equal( inputs[2].GetAttribute( "id" ), invocation.Arguments[1] as string );
        }, TestExtensions.WaitTime );
    }

    [Fact]
    public async Task MovesFocusToLastFilledSlotWhenPasteFillsRemainingSlots()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add( x => x.Digits, 4 ) );

        var inputs = comp.FindAll( "input" );

        await inputs[0].InputAsync( "1234" );

        inputs = comp.FindAll( "input" );

        Assert.Equal( "1234", comp.Instance.Value );
        Assert.Equal( "1", inputs[0].GetAttribute( "value" ) );
        Assert.Equal( "2", inputs[1].GetAttribute( "value" ) );
        Assert.Equal( "3", inputs[2].GetAttribute( "value" ) );
        Assert.Equal( "4", inputs[3].GetAttribute( "value" ) );

        comp.WaitForAssertion( () =>
        {
            var invocation = JSInterop.VerifyInvoke( "focus" );

            Assert.Equal( inputs[3].GetAttribute( "id" ), invocation.Arguments[1] as string );
        }, TestExtensions.WaitTime );
    }

    [Fact]
    public void RendersFeedbackAsSiblingOfTheInputContainer()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add<RenderFragment>( x => x.Feedback, builder =>
            {
                builder.OpenElement( 0, "div" );
                builder.AddAttribute( 1, "class", "invalid-feedback" );
                builder.AddContent( 2, "Invalid code." );
                builder.CloseElement();
            } ) );

        Assert.NotNull( comp.Find( ".b-one-time-input + .invalid-feedback" ) );
    }

    [Fact]
    public async Task ClearsCurrentSlotAndMovesFocusToPreviousSlotOnBackspace()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add( x => x.Digits, 4 )
            .Add( x => x.Value, "12" ) );

        var inputs = comp.FindAll( "input" );

        await inputs[1].KeyDownAsync( new() { Code = "Backspace" } );

        inputs = comp.FindAll( "input" );

        Assert.Equal( "1", comp.Instance.Value );
        Assert.Equal( "1", inputs[0].GetAttribute( "value" ) );
        Assert.True( string.IsNullOrEmpty( inputs[1].GetAttribute( "value" ) ) );

        comp.WaitForAssertion( () =>
        {
            var invocation = JSInterop.VerifyInvoke( "focus" );

            Assert.Equal( inputs[0].GetAttribute( "id" ), invocation.Arguments[1] as string );
        }, TestExtensions.WaitTime );
    }

    [Fact]
    public void SynchronizesSlotsFromBoundValue()
    {
        var comp = RenderComponent<OneTimeInput>( parameters => parameters
            .Add( x => x.Digits, 4 )
            .Add( x => x.Value, "12" ) );

        Assert.Equal( new[] { "1", "2", null, null }, ReadSlotValues( comp ) );

        comp.SetParametersAndRender( parameters => parameters
            .Add( x => x.Digits, 4 )
            .Add( x => x.Value, "9876" ) );

        Assert.Equal( new[] { "9", "8", "7", "6" }, ReadSlotValues( comp ) );
    }

    [Fact]
    public async Task AppliesParentValidationStateToAllSlots()
    {
        var comp = Render( builder =>
        {
            builder.OpenComponent<Validation>( 0 );
            builder.AddAttribute( 1, nameof( Validation.Validator ), (Action<ValidatorEventArgs>)ValidationRule.IsNotEmpty );
            builder.AddAttribute( 2, nameof( Validation.ChildContent ), (RenderFragment)( childBuilder =>
            {
                childBuilder.OpenComponent<OneTimeInput>( 0 );
                childBuilder.AddAttribute( 1, nameof( OneTimeInput.Digits ), 4 );
                childBuilder.CloseComponent();
            } ) );
            builder.CloseComponent();
        } );

        var container = comp.Find( ".b-one-time-input" );
        var inputs = comp.FindAll( "input" );

        Assert.DoesNotContain( "is-invalid", container.GetAttribute( "class" ) ?? string.Empty );
        Assert.All( inputs, input => Assert.Contains( "is-invalid", input.GetAttribute( "class" ) ?? string.Empty ) );

        await inputs[0].InputAsync( "1" );

        container = comp.Find( ".b-one-time-input" );
        inputs = comp.FindAll( "input" );

        Assert.DoesNotContain( "is-valid", container.GetAttribute( "class" ) ?? string.Empty );
        Assert.All( inputs, input => Assert.Contains( "is-valid", input.GetAttribute( "class" ) ?? string.Empty ) );
    }

    [Fact]
    public async Task AppliesPatternValidationToTheCombinedValue()
    {
        var comp = Render( builder =>
        {
            builder.OpenComponent<Validation>( 0 );
            builder.AddAttribute( 1, nameof( Validation.UsePattern ), true );
            builder.AddAttribute( 2, nameof( Validation.ChildContent ), (RenderFragment)( childBuilder =>
            {
                childBuilder.OpenComponent<OneTimeInput>( 0 );
                childBuilder.AddAttribute( 1, nameof( OneTimeInput.Digits ), 4 );
                childBuilder.AddAttribute( 2, nameof( OneTimeInput.Pattern ), "[0-9]{4}" );
                childBuilder.CloseComponent();
            } ) );
            builder.CloseComponent();
        } );

        var container = comp.Find( ".b-one-time-input" );
        var inputs = comp.FindAll( "input" );

        Assert.DoesNotContain( "is-invalid", container.GetAttribute( "class" ) ?? string.Empty );
        Assert.All( inputs, input => Assert.Contains( "is-invalid", input.GetAttribute( "class" ) ?? string.Empty ) );

        await inputs[0].InputAsync( "1234" );

        container = comp.Find( ".b-one-time-input" );
        inputs = comp.FindAll( "input" );

        Assert.DoesNotContain( "is-valid", container.GetAttribute( "class" ) ?? string.Empty );
        Assert.All( inputs, input => Assert.Contains( "is-valid", input.GetAttribute( "class" ) ?? string.Empty ) );
    }

    private static string[] ReadSlotValues( IRenderedFragment fragment )
        => fragment.FindAll( "input" )
            .Select( input => string.IsNullOrEmpty( input.GetAttribute( "value" ) ) ? null : input.GetAttribute( "value" ) )
            .ToArray();
}