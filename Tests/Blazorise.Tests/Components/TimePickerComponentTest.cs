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
    private const string MobileUserAgent = "Mozilla/5.0 (Linux; Android 15; Pixel 9) AppleWebKit/537.36 Chrome/131.0.0.0 Mobile Safari/537.36";

    public TimePickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities( MobileUserAgent, mobileDevice: true );
        JSInterop.AddBlazoriseDocumentObserver();
    }

    [Fact]
    public async Task MobileModeUsesNativeTimeInput()
    {
        // test
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Value, new TimeSpan( 9, 5, 7 ) )
            .Add( x => x.Min, new TimeSpan( 8, 30, 0 ) )
            .Add( x => x.Max, new TimeSpan( 17, 45, 0 ) )
            .Add( x => x.Seconds, true )
            .Add( x => x.DisableMobile, false ) );
        IElement input = comp.Find( "input" );

        await input.ChangeAsync( new ChangeEventArgs { Value = "10:15:30" } );
        input = comp.Find( "input" );

        // validate
        Assert.Equal( "time", input.GetAttribute( "type" ) );
        Assert.Equal( "10:15:30", input.GetAttribute( "value" ) );
        Assert.Equal( new TimeSpan( 10, 15, 30 ), comp.Instance.Value );
        Assert.Equal( "08:30:00", input.GetAttribute( "min" ) );
        Assert.Equal( "17:45:00", input.GetAttribute( "max" ) );
        Assert.Equal( "any", input.GetAttribute( "step" ) );
        Assert.False( input.HasAttribute( "role" ) );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task MobileOpenUsesBrowserPicker()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.DisableMobile, false ) );

        // test
        await comp.InvokeAsync( () => comp.Instance.OpenAsync().AsTask() );

        // validate
        JSInterop.VerifyInvoke( "showPicker", 1 );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
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
        Assert.Equal(
            $"[id=\"{comp.Instance.PickerContainerId}\"], [id=\"{comp.Instance.MenuId}\"]",
            subscription.ExcludeSelector );
    }

    [Fact]
    public async Task OpeningUnsetPickerUsesDefaultTimeAndMeridiem()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>();

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "12", comp.Find( ".timepicker-input" ).GetAttribute( "value" ) );
        Assert.Equal( "00", comp.FindAll( ".timepicker-input" )[1].GetAttribute( "value" ) );
        Assert.Equal( "PM", comp.Find( ".timepicker-meridiem" ).TextContent.Trim() );
        Assert.Contains( "timepicker-meridiem-pm", comp.Find( ".timepicker-meridiem" ).ClassList );
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

    [Theory]
    [InlineData( "1333", 13, 33, "13:33" )]
    [InlineData( "13", 13, 0, "13:00" )]
    public async Task CompactNumericInputParsesEditableText( string text, int hour, int minute, string expectedText )
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>();
        IElement input = comp.Find( "input" );

        // test
        await input.InputAsync( new ChangeEventArgs { Value = text } );
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = text } );

        // validate
        Assert.Equal( new TimeSpan( hour, minute, 0 ), comp.Instance.Value );
        Assert.Equal( expectedText, comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Theory]
    [InlineData( 12, 21, "21:33" )]
    [InlineData( 9, 9, "09:33" )]
    public async Task CompactNumericInputUsesPickerMeridiemInTwelveHourMode( int defaultHour, int expectedHour, string expectedText )
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.DefaultHour, defaultHour ) );
        IElement input = comp.Find( "input" );

        // test
        await input.InputAsync( new ChangeEventArgs { Value = "933" } );
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "933" } );

        // validate
        Assert.Equal( new TimeSpan( expectedHour, 33, 0 ), comp.Instance.Value );
        Assert.Equal( expectedText, comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task TwelveHourMenuAcceptsTwentyFourHourValue()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>();

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( ".timepicker-input" ).ChangeAsync( new ChangeEventArgs { Value = "16" } );

        // validate
        Assert.Equal( new TimeSpan( 16, 0, 0 ), comp.Instance.Value );
        Assert.Equal( "16:00", comp.Find( "input" ).GetAttribute( "value" ) );
        Assert.Equal( "04", comp.Find( ".timepicker-input" ).GetAttribute( "value" ) );
        Assert.Equal( "PM", comp.Find( ".timepicker-meridiem" ).TextContent.Trim() );
    }

    [Fact]
    public async Task CompactNumericInputUsesLiteralHourInTwentyFourHourMode()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.TimeAs24hr, true ) );
        IElement input = comp.Find( "input" );

        // test
        await input.InputAsync( new ChangeEventArgs { Value = "933" } );
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "933" } );

        // validate
        Assert.Equal( new TimeSpan( 9, 33, 0 ), comp.Instance.Value );
        Assert.Equal( "09:33", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task CompactNumericInputParsesSeconds()
    {
        // setup
        IRenderedComponent<TimePicker<TimeSpan?>> comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( x => x.Seconds, true ) );
        IElement input = comp.Find( "input" );

        // test
        await input.InputAsync( new ChangeEventArgs { Value = "133355" } );
        await comp.Find( "input" ).ChangeAsync( new ChangeEventArgs { Value = "133355" } );

        // validate
        Assert.Equal( new TimeSpan( 13, 33, 55 ), comp.Instance.Value );
        Assert.Equal( "13:33:55", comp.Find( "input" ).GetAttribute( "value" ) );
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