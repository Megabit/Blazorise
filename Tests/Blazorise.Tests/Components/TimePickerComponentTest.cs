using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Blazorise.Tests.Components;

public class TimePickerComponentTest : BunitContext
{
    public TimePickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDocumentObserver();
    }

    [Fact]
    public void InlineMenuUsesProviderOwnedClasses()
    {
        // test
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Value, new TimeSpan( 9, 30, 0 ) )
            .Add( x => x.Inline, true )
            .Add( x => x.Seconds, true ) );

        // validate
        Assert.NotNull( comp.Find( ".timepicker" ) );
        Assert.NotNull( comp.Find( ".timepicker-menu-inline" ) );
        Assert.Equal( 3, comp.FindAll( ".timepicker-input" ).Count );
        Assert.DoesNotContain( "b-timepicker", comp.Markup );
        Assert.DoesNotContain( "flatpickr", comp.Markup );
    }

    [Fact]
    public async Task MenuKeyboardNavigationAdjustsFocusedTime()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Inline, true )
            .Add( x => x.DefaultHour, 12 ) );

        // test
        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "ArrowUp" } );

        // validate
        Assert.Equal( new TimeSpan( 13, 0, 0 ), comp.Instance.Value );
    }

    [Fact]
    public async Task ClickingInputOpensMenuWithoutMovingFocusToMenu()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>();

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.NotNull( comp.Find( "[role='dialog']" ) );
        Assert.False( comp.Instance.FocusMenuOnOpen );
        Assert.All(
            comp.FindAll( "[role='dialog'] button, [role='dialog'] input" ),
            element => Assert.Equal( "-1", element.GetAttribute( "tabindex" ) ) );

        var invocation = JSInterop.VerifyInvoke( "addSubscription", 1 );
        DocumentObserverJsSubscription subscription = Assert.IsType<DocumentObserverJsSubscription>( invocation.Arguments[0] );

        Assert.Equal( "pointerdown", Assert.Single( subscription.EventNames ) );
        Assert.Equal( $"[id=\"{comp.Instance.PickerContainerId}\"]", subscription.ExcludeSelector );
    }

    [Fact]
    public async Task TabFromInputClosesMenu()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>();
        IElement input = comp.Find( "input" );

        // test
        await input.ClickAsync( new MouseEventArgs() );
        await input.KeyDownAsync( new KeyboardEventArgs { Key = "Tab" } );

        // validate
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task DisplayFormatParsesEditableText()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>();
        IElement input = comp.Find( "input" );

        // test
        await input.InputAsync( new ChangeEventArgs { Value = "12:34" } );
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "12:34" } );

        // validate
        Assert.Equal( new TimeSpan( 12, 34, 0 ), comp.Instance.Value );
    }

    [Fact]
    public void DisplayFormatPreservesPreviousPickerTokenSemantics()
    {
        // test
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Value, new TimeSpan( 9, 5, 0 ) )
            .Add( x => x.DisplayFormat, "H:m" ) );

        // validate
        Assert.Equal( "09:05", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task MenuSelectionHonorsMaximumTime()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Inline, true )
            .Add( x => x.Value, new TimeSpan( 16, 30, 0 ) )
            .Add( x => x.Max, new TimeSpan( 17, 0, 0 ) )
            .Add( x => x.TimeAs24hr, true ) );
        IElement hourInput = comp.Find( ".timepicker-input" );

        // test
        await hourInput.ChangeAsync( new ChangeEventArgs { Value = "20" } );

        // validate
        Assert.Equal( new TimeSpan( 17, 0, 0 ), comp.Instance.Value );
    }

    [Fact]
    public async Task MeridiemButtonUpdatesTime()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Inline, true )
            .Add( x => x.Value, new TimeSpan( 9, 15, 0 ) ) );

        // test
        await comp.Find( ".timepicker-meridiem" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( new TimeSpan( 21, 15, 0 ), comp.Instance.Value );
    }
}