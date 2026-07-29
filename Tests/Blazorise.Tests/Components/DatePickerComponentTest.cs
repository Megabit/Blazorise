using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Blazorise.Modules;
using Blazorise.Utilities;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Xunit;

namespace Blazorise.Tests.Components;

public class DatePickerComponentTest : BunitContext
{
    private const string MobileUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 Mobile/15E148";

    public DatePickerComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities( MobileUserAgent, mobileDevice: true );
        JSInterop.AddBlazoriseInputMask();
        JSInterop.AddBlazoriseDocumentObserver();
    }

    [Theory]
    [InlineData( "Mozilla/5.0 Mobile" )]
    [InlineData( "Opera Mobi" )]
    [InlineData( "Silk/3.13" )]
    [InlineData( "Kindle/3.0" )]
    [InlineData( "Windows Phone 10.0" )]
    [InlineData( "PlayBook" )]
    [InlineData( "BB10" )]
    [InlineData( "MeeGo" )]
    [InlineData( "Tizen 6.0" )]
    [InlineData( "Puffin/10.0" )]
    public void MobileDetectorUsesCompleteCompatibilityUserAgentPattern( string userAgent )
    {
        Assert.True( MobileDeviceDetector.IsMobile( userAgent ) );
    }

    [Fact]
    public void DesktopUserAgentDoesNotMatchMobilePattern()
    {
        Assert.False( MobileDeviceDetector.IsMobile( "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/131.0.0.0" ) );
    }

    [Fact]
    public async Task MobileModeUsesNativeDateInput()
    {
        // setup
        DateTime value = new( 2026, 7, 27 );

        // test
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.DisplayFormat, "dd.MM.yyyy" )
            .Add( x => x.DisableMobile, false ) );
        IElement input = comp.Find( "input" );

        await input.ChangeAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "2026-07-28" } );
        input = comp.Find( "input" );

        // validate
        Assert.Equal( "date", input.GetAttribute( "type" ) );
        Assert.Equal( "2026-07-28", input.GetAttribute( "value" ) );
        Assert.Equal( new DateTime( 2026, 7, 28 ), comp.Instance.Value );
        Assert.False( input.HasAttribute( "role" ) );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public void MobileModeRetainsBlazorCalendarForDisabledDates()
    {
        // test
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.DisableMobile, false )
            .Add( x => x.DisabledDates, new[] { new DateTime( 2026, 7, 28 ) } ) );
        IElement input = comp.Find( "input" );

        // validate
        Assert.Equal( "text", input.GetAttribute( "type" ) );
        Assert.Equal( "combobox", input.GetAttribute( "role" ) );
    }

    [Fact]
    public void MobileModeRetainsBlazorCalendarWhenEnabledDatesIsDefined()
    {
        // test
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.DisableMobile, false )
            .Add( x => x.EnabledDates, Array.Empty<DateTime>() ) );

        // validate
        Assert.Equal( "text", comp.Find( "input" ).GetAttribute( "type" ) );
    }

    [Fact]
    public void MobileModeRetainsBlazorCalendarWhenDisabledDaysIsDefined()
    {
        // test
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.DisableMobile, false )
            .Add( x => x.DisabledDays, Array.Empty<DayOfWeek>() ) );

        // validate
        Assert.Equal( "text", comp.Find( "input" ).GetAttribute( "type" ) );
    }

    [Fact]
    public async Task MobileOpenUsesBrowserPicker()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.DisableMobile, false ) );

        // test
        await comp.InvokeAsync( () => comp.Instance.OpenAsync().AsTask() );

        // validate
        JSInterop.VerifyInvoke( "showPicker", 1 );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public void InlineCalendarUsesProviderOwnedClasses()
    {
        // setup
        DateTime value = new( 2026, 7, 27 );

        // test
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.Inline, true ) );

        // validate
        Assert.NotNull( comp.Find( ".datepicker" ) );
        Assert.NotNull( comp.Find( ".datepicker-calendar-inline" ) );
        Assert.Equal( 42, comp.FindAll( ".datepicker-day" ).Count );
        Assert.DoesNotContain( "b-datepicker", comp.Markup );
        Assert.DoesNotContain( "flatpickr", comp.Markup );
    }

    [Fact]
    public async Task CalendarKeyboardNavigationSelectsFocusedDate()
    {
        // setup
        DateTime value = new( 2026, 7, 27 );
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.Inline, true ) );

        // test
        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "ArrowRight" } );
        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "Enter" } );

        // validate
        Assert.Equal( new DateTime( 2026, 7, 28 ), comp.Instance.Value );
    }

    [Fact]
    public async Task ClickingInputOpensCalendarWithoutMovingFocusToCalendar()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>();

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.NotNull( comp.Find( "[role='dialog']" ) );
        Assert.False( comp.Instance.FocusCalendarOnOpen );
        Assert.All(
            comp.FindAll( "[role='dialog'] button, [role='dialog'] select, [role='dialog'] input" ),
            element => Assert.Equal( "-1", element.GetAttribute( "tabindex" ) ) );

        var invocation = JSInterop.VerifyInvoke( "addSubscription", 1 );
        DocumentObserverJsSubscription subscription = Assert.IsType<DocumentObserverJsSubscription>( invocation.Arguments[0] );

        Assert.Equal( "pointerdown", Assert.Single( subscription.EventNames ) );
        Assert.Equal(
            $"[id=\"{comp.Instance.PickerContainerId}\"], [id=\"{comp.Instance.CalendarId}\"]",
            subscription.ExcludeSelector );
    }

    [Fact]
    public async Task TabFromInputClosesCalendar()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>();
        IElement input = comp.Find( "input" );

        // test
        await input.ClickAsync( new MouseEventArgs() );
        await input.KeyDownAsync( new KeyboardEventArgs { Key = "Tab" } );

        // validate
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task InputFormatUsesExistingMaskAndParsesText()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.InputFormat, "dd.MM.yyyy" )
            .Add( x => x.DisplayFormat, "yyyy-MM-dd" ) );
        IElement input = comp.Find( "input" );

        // test
        await input.FocusAsync( new FocusEventArgs() );
        await input.ChangeAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "27.07.2026" } );

        // validate
        var invocation = JSInterop.VerifyInvoke( "initialize", 1 );
        InputMaskJSOptions options = Assert.IsType<InputMaskJSOptions>( invocation.Arguments[3] );

        Assert.Equal( "datetime", options.Alias );
        Assert.Equal( "dd.mm.yyyy", options.InputFormat );
        Assert.Equal( "_", options.MaskPlaceholder );
        Assert.True( options.ShowMaskOnFocus );
        Assert.True( options.ShowMaskOnHover );
        Assert.True( options.DispatchChangeOnComplete );
        JSInterop.VerifyInvoke( "destroy", 1 );
        JSInterop.VerifyInvoke( "setTextValue", 1 );
        Assert.Equal( new DateTime( 2026, 7, 27 ), comp.Instance.Value );
        Assert.Equal( "2026-07-27", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public void SingleTokenDisplayFormatPreservesPreviousPickerSemantics()
    {
        // setup
        DateTime value = new( 2026, 7, 9 );

        // test
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.DisplayFormat, "d" ) );

        // validate
        Assert.Equal( "9", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public void DateTimeDefaultDisplayFormatPreservesPreviousPickerSemantics()
    {
        // setup
        DateTime first = new( 2026, 7, 26, 22, 22, 21 );
        DateTime second = new( 2026, 8, 1, 22, 22, 21 );

        // test
        IRenderedComponent<DatePicker<DateTime>> single = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, first )
            .Add( x => x.InputMode, DateInputMode.DateTime ) );
        IRenderedComponent<DatePicker<DateTime[]>> range = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.Value, new[] { first, second } )
            .Add( x => x.InputMode, DateInputMode.DateTime )
            .Add( x => x.SelectionMode, DateInputSelectionMode.Range ) );
        IRenderedComponent<DatePicker<DateTime[]>> multiple = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.Value, new[] { first, second } )
            .Add( x => x.InputMode, DateInputMode.DateTime )
            .Add( x => x.SelectionMode, DateInputSelectionMode.Multiple ) );

        // validate
        Assert.Equal( "2026-07-26 22:22", single.Find( "input" ).GetAttribute( "value" ) );
        Assert.Equal( "2026-07-26 22:22 to 2026-08-01 22:22", range.Find( "input" ).GetAttribute( "value" ) );
        Assert.Equal( "2026-07-26 22:22, 2026-08-01 22:22", multiple.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public void DisabledDateRendersAsDisabledCalendarCell()
    {
        // setup
        DateTime value = new( 2026, 7, 27 );

        // test
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.Inline, true )
            .Add( x => x.DisabledDates, new[] { new DateTime( 2026, 7, 28 ) } ) );

        // validate
        Assert.True( comp.Find( "[id$='day-20260728']" ).HasAttribute( "disabled" ) );
    }

    [Fact]
    public void DateTimeMenuUsesTwoDigitTimeValues()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 7, 27, 7, 4, 0 ) )
            .Add( x => x.InputMode, DateInputMode.DateTime )
            .Add( x => x.Inline, true ) );

        // validate
        Assert.Equal( "07", comp.FindAll( ".datepicker-time-input" )[0].GetAttribute( "value" ) );
        Assert.Equal( "04", comp.FindAll( ".datepicker-time-input" )[1].GetAttribute( "value" ) );
    }

    [Fact]
    public async Task RangeSelectionCommitsOrderedDates()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime[]>> comp = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.Value, new[] { new DateTime( 2026, 7, 27 ) } )
            .Add( x => x.SelectionMode, DateInputSelectionMode.Range )
            .Add( x => x.Inline, true ) );

        // test
        await comp.Find( "[id$='day-20260729']" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( "[id$='day-20260727']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal(
            new[] { new DateTime( 2026, 7, 27 ), new DateTime( 2026, 7, 29 ) },
            comp.Instance.Value );
    }

    [Fact]
    public void RenderDateTimeTest()
    {
        // setup
        var defDate = new DateTime();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""text""";
        var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-event-initially-null-result""></span>";

        // test
        var comp = Render<DatePickerComponent>();

        // validate
        Assert.Contains( dateOpen, comp.Markup );
        Assert.Contains( dateClose, comp.Markup );
        Assert.Contains( dateType, comp.Markup );
        Assert.Contains( dateOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#date-event-initially-undefined" ) );
        Assert.NotNull( comp.Find( "#date-control" ) );
        Assert.NotNull( comp.Find( "#date-event-initially-undefined-result" ) );

        Assert.Contains( nullableOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#nullable-date-event-initially-null" ) );
        Assert.NotNull( comp.Find( "#nullable-date-control" ) );
        Assert.NotNull( comp.Find( "#nullable-date-event-initially-null-result" ) );
    }

    [Fact]
    public void RenderDateOnlyTest()
    {
        // setup
        var defDate = new DateOnly();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""text""";
        var dateOutput = @"<span id=""date-only-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-only-event-initially-null-result""></span>";

        // test
        var comp = Render<DatePickerComponent>();

        // validate
        Assert.Contains( dateOpen, comp.Markup );
        Assert.Contains( dateClose, comp.Markup );
        Assert.Contains( dateType, comp.Markup );
        Assert.Contains( dateOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#date-only-event-initially-undefined" ) );
        Assert.NotNull( comp.Find( "#date-only-control" ) );
        Assert.NotNull( comp.Find( "#date-only-event-initially-undefined-result" ) );

        Assert.Contains( nullableOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#nullable-date-only-event-initially-null" ) );
        Assert.NotNull( comp.Find( "#nullable-date-only-control" ) );
        Assert.NotNull( comp.Find( "#nullable-date-only-event-initially-null-result" ) );
    }

    [Fact]
    public void RenderDateTimeOffsetTest()
    {
        // setup
        var defDate = new DateTimeOffset();
        var dateOpen = "<input";
        var dateClose = "</input>";
        var dateType = @"type=""text""";
        var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + defDate.ToString() + "</span>";
        var nullableOutput = @"<span id=""nullable-date-offset-event-initially-null-result""></span>";

        // test
        var comp = Render<DatePickerComponent>();

        // validate
        Assert.Contains( dateOpen, comp.Markup );
        Assert.Contains( dateClose, comp.Markup );
        Assert.Contains( dateType, comp.Markup );
        Assert.Contains( dateOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#date-offset-event-initially-undefined" ) );
        Assert.NotNull( comp.Find( "#date-offset-control" ) );
        Assert.NotNull( comp.Find( "#date-offset-event-initially-undefined-result" ) );

        Assert.Contains( nullableOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#nullable-date-offset-event-initially-null" ) );
        Assert.NotNull( comp.Find( "#nullable-date-offset-control" ) );
        Assert.NotNull( comp.Find( "#nullable-date-offset-event-initially-null-result" ) );
    }

    [Fact]
    public void SetDateTime()
    {
        // setup
        var dateOutput = @"<span id=""date-event-initially-undefined-result"">" + new DateTime( 1970, 5, 3 ).ToString() + "</span>";
        var comp = Render<DatePickerComponent>();

        // test
        comp.Instance.DateValue = new( 1970, 5, 3 );
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetNullableDateTime()
    {
        // setup
        var dateOutput = @"<span id=""nullable-date-event-initially-null-result"">" + new DateTime( 1970, 5, 3 ).ToString() + "</span>";
        var comp = Render<DatePickerComponent>();

        // test
        comp.Instance.NullableDateValue = new DateTime( 1970, 5, 3 );
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetDateOnly()
    {
        // setup
        var dateonly = new DateOnly( 2020, 4, 13 );
        var dateOutput = @"<span id=""date-only-event-initially-undefined-result"">" + dateonly.ToString() + "</span>";
        var comp = Render<DatePickerComponent>();

        // test
        comp.Instance.DateOnlyValue = dateonly;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetNullableDateOnly()
    {
        // setup
        var dateonly = new DateOnly( 2020, 4, 13 );
        var dateOutput = @"<span id=""nullable-date-only-event-initially-null-result"">" + dateonly.ToString() + "</span>";
        var comp = Render<DatePickerComponent>();

        // test
        comp.Instance.NullableDateOnlyValue = dateonly;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetDateTimeOffset()
    {
        // setup
        var offset = new DateTimeOffset( new( 2020, 4, 13 ) );
        var dateOutput = @"<span id=""date-offset-event-initially-undefined-result"">" + offset.ToString() + "</span>";
        var comp = Render<DatePickerComponent>();

        // test
        comp.Instance.OffsetValue = offset;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }

    [Fact]
    public void SetNullableDateTimeOffset()
    {
        // setup
        var offset = new DateTimeOffset( new( 2020, 4, 13 ) );
        var dateOutput = @"<span id=""nullable-date-offset-event-initially-null-result"">" + offset.ToString() + "</span>";
        var comp = Render<DatePickerComponent>();

        // test
        comp.Instance.NullableOffsetValue = offset;
        comp.Render();

        // validate
        Assert.Contains( dateOutput, comp.Markup );
    }
}