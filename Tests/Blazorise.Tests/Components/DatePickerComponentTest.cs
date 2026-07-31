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
    public void WeekDisplayFormatUsesEnglishOrdinalSuffix()
    {
        Assert.Equal(
            "2026-41st",
            WeekDateFormat.Format(
                new DateTime( 2026, 10, 8 ),
                WeekDateFormat.DefaultDisplayFormat,
                System.Globalization.CultureInfo.GetCultureInfo( "en-US" ) ) );
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
    public async Task SharedCalendarNavigationSupportsMonthAndYearChanges()
    {
        // setup
        DateTime value = new( 2026, 7, 27 );
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, value )
            .Add( x => x.Inline, true ) );

        // validate
        Assert.Single( comp.FindAll( "button[aria-label='Previous year']" ) );
        Assert.Single( comp.FindAll( "button[aria-label='Previous month']" ) );
        Assert.Single( comp.FindAll( "button[aria-label='Next month']" ) );
        Assert.Single( comp.FindAll( "button[aria-label='Next year']" ) );
        Assert.Collection(
            comp.FindComponents<Icon>(),
            icon => Assert.Equal( IconName.ChevronDoubleLeft, icon.Instance.Name ),
            icon => Assert.Equal( IconName.ChevronLeft, icon.Instance.Name ),
            icon => Assert.Equal( IconName.ChevronRight, icon.Instance.Name ),
            icon => Assert.Equal( IconName.ChevronDoubleRight, icon.Instance.Name ) );

        // test
        await comp.Find( "button[aria-label='Next year']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2027", comp.Find( "input[aria-label='Year']" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task PopupCalendarNavigationRefreshesImmediately()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 7, 27 ) ) );

        await comp.Find( "input[role='combobox']" ).ClickAsync( new MouseEventArgs() );

        // test
        await comp.Find( "button[aria-label='Next month']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "8", comp.Find( "select[aria-label='Month']" ).GetAttribute( "value" ) );

        // test
        await comp.Find( "button[aria-label='Next year']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2027", comp.Find( "input[aria-label='Year']" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task PendingRangeSelectionRefreshesImmediately()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime[]>> comp = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.Value, new[] { new DateTime( 2026, 7, 27 ) } )
            .Add( x => x.SelectionMode, DateInputSelectionMode.Range )
            .Add( x => x.Inline, true ) );

        // test
        await comp.Find( "[id$='day-20260728']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Contains( "datepicker-day-range-start", comp.Find( "[id$='day-20260728']" ).ClassList );
    }

    [Fact]
    public void MonthModeDoesNotDuplicateYearNavigation()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.Inline, true )
            .Add( x => x.InputMode, DateInputMode.Month ) );

        // validate
        Assert.Single( comp.FindAll( "button[aria-label='Previous year']" ) );
        Assert.Single( comp.FindAll( "button[aria-label='Next year']" ) );
        Assert.Empty( comp.FindAll( "button[aria-label='Previous month']" ) );
        Assert.Empty( comp.FindAll( "button[aria-label='Next month']" ) );
        Assert.Collection(
            comp.FindComponents<Icon>(),
            icon => Assert.Equal( IconName.ChevronDoubleLeft, icon.Instance.Name ),
            icon => Assert.Equal( IconName.ChevronDoubleRight, icon.Instance.Name ) );
    }

    [Fact]
    public async Task MonthModeNavigatesThroughYearsAndDecadesBeforeSelectingMonth()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 7, 1 ) )
            .Add( x => x.Inline, true )
            .Add( x => x.InputMode, DateInputMode.Month ) );

        // test
        await comp.Find( ".datepicker-title > button" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2020-2029", comp.Find( ".datepicker-title > button" ).TextContent.Trim() );
        Assert.Collection(
            comp.FindAll( ".datepicker-months[data-calendar-view='year'] > button" ),
            year => Assert.Equal( "2019", year.TextContent.Trim() ),
            year => Assert.Equal( "2020", year.TextContent.Trim() ),
            year => Assert.Equal( "2021", year.TextContent.Trim() ),
            year => Assert.Equal( "2022", year.TextContent.Trim() ),
            year => Assert.Equal( "2023", year.TextContent.Trim() ),
            year => Assert.Equal( "2024", year.TextContent.Trim() ),
            year => Assert.Equal( "2025", year.TextContent.Trim() ),
            year => Assert.Equal( "2026", year.TextContent.Trim() ),
            year => Assert.Equal( "2027", year.TextContent.Trim() ),
            year => Assert.Equal( "2028", year.TextContent.Trim() ),
            year => Assert.Equal( "2029", year.TextContent.Trim() ),
            year => Assert.Equal( "2030", year.TextContent.Trim() ) );

        // test
        await comp.Find( "button[aria-label='Next decade']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2030-2039", comp.Find( ".datepicker-title > button" ).TextContent.Trim() );

        // test
        await comp.Find( "button[aria-label='Previous decade']" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( ".datepicker-title > button" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2000-2099", comp.Find( ".datepicker-title > span" ).TextContent.Trim() );
        Assert.Equal( "1990-1999", comp.Find( ".datepicker-months[data-calendar-view='decade'] > button" ).TextContent.Trim() );

        // test
        await comp.Find( "button[aria-label='Next century']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2100-2199", comp.Find( ".datepicker-title > span" ).TextContent.Trim() );

        // test
        await comp.Find( "button[aria-label='Previous century']" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( "button[aria-label='2040-2049']" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( "button[aria-label='2045']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( "2045", comp.Find( ".datepicker-title > button" ).TextContent.Trim() );
        Assert.Equal( 12, comp.FindAll( ".datepicker-months[data-calendar-view='month'] > button" ).Count );
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
    public async Task WeekModeSelectsAndFormatsCompleteIsoWeek()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 10, 8 ) )
            .Add( x => x.InputMode, DateInputMode.Week )
            .Add( x => x.DisplayFormat, "yyyy-'W'ww" )
            .Add( x => x.Inline, true ) );

        // validate
        Assert.Equal( "2026-W41", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );
        Assert.Equal( 6, comp.FindAll( ".datepicker-week .datepicker-week-number" ).Count );

        IElement selectedWeek = comp.Find( ".datepicker-week[data-week-selected='true']" );
        Assert.Equal( 7, selectedWeek.QuerySelectorAll( ".datepicker-day-selected" ).Length );

        // test
        await comp.Find( "[id$='day-20261020']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( new DateTime( 2026, 10, 19 ), comp.Instance.Value );
        Assert.Equal( "2026-W43", comp.Find( "input[type='text']" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task WeekModeUsesFirstDayOfWeekForLayoutWithoutChangingIsoSelection()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 10, 8 ) )
            .Add( x => x.InputMode, DateInputMode.Week )
            .Add( x => x.FirstDayOfWeek, DayOfWeek.Sunday )
            .Add( x => x.Inline, true ) );

        // validate
        Assert.Equal( "Sun", comp.Find( ".datepicker-weekday" ).TextContent );
        Assert.Equal( 7, comp.FindAll( ".datepicker-day-selected" ).Count );
        Assert.NotEqual(
            comp.Find( "[id$='day-20261005']" ).ParentElement,
            comp.Find( "[id$='day-20261011']" ).ParentElement );
        Assert.Equal( "41", comp.Find( ".datepicker-week[data-week-selected='true'] .datepicker-week-number" ).TextContent );

        // test
        await comp.Find( "[id$='day-20261007']" ).TriggerEventAsync( "onmouseover", new MouseEventArgs() );

        // validate
        Assert.Equal( 7, comp.FindAll( ".datepicker-day[data-week-hovered='true']" ).Count );
        Assert.Equal( "41", comp.Find( ".datepicker-week-number[data-week-hovered='true']" ).TextContent );
    }

    [Fact]
    public async Task WeekModeParsesOrdinalWeekInput()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.InputMode, DateInputMode.Week )
            .Add( x => x.DisplayFormat, "yyyy-'W'ww" ) );

        // test
        await comp.Find( "input" ).ChangeAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "2026-41st" } );

        // validate
        Assert.Equal( new DateTime( 2026, 10, 5 ), comp.Instance.Value );
        Assert.Equal( "2026-W41", comp.Find( "input" ).GetAttribute( "value" ) );
    }

    [Fact]
    public async Task WeekModeKeyboardNavigationMovesByCompleteWeeks()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 10, 8 ) )
            .Add( x => x.InputMode, DateInputMode.Week )
            .Add( x => x.Inline, true ) );

        // test
        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "ArrowDown" } );
        await comp.Find( "[role='dialog']" ).KeyDownAsync( new KeyboardEventArgs { Key = "Enter" } );

        // validate
        Assert.Equal( new DateTime( 2026, 10, 12 ), comp.Instance.Value );
    }

    [Fact]
    public async Task MobileWeekModeUsesNativeWeekInput()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 10, 8 ) )
            .Add( x => x.InputMode, DateInputMode.Week )
            .Add( x => x.DisableMobile, false ) );
        IElement input = comp.Find( "input" );

        // validate
        Assert.Equal( "week", input.GetAttribute( "type" ) );
        Assert.Equal( "2026-W41", input.GetAttribute( "value" ) );

        // test
        await input.ChangeAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs { Value = "2026-W42" } );

        // validate
        Assert.Equal( new DateTime( 2026, 10, 12 ), comp.Instance.Value );
    }

    [Fact]
    public async Task WeekInputFormatUsesNumericWeekMask()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.InputMode, DateInputMode.Week )
            .Add( x => x.InputFormat, "yyyy-ww" ) );

        // test
        await comp.Find( "input" ).FocusAsync( new FocusEventArgs() );

        // validate
        JSRuntimeInvocation inputMaskInitialization = Assert.Single(
            JSInterop.Invocations["initialize"],
            invocation => invocation.Arguments.Count > 3 && invocation.Arguments[3] is InputMaskJSOptions );
        InputMaskJSOptions options = Assert.IsType<InputMaskJSOptions>( inputMaskInitialization.Arguments[3] );

        Assert.Null( options.Alias );
        Assert.Equal( "9999-99", options.Mask );
    }

    [Fact]
    public async Task SelectingDateRestoresInputFocusDuringCloseRender()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime>> comp = Render<DatePicker<DateTime>>( parameters => parameters
            .Add( x => x.Value, new DateTime( 2026, 7, 27 ) ) );

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( "[id$='day-20260728']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
        JSInterop.VerifyInvoke( "focus", 1 );
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

        Assert.Equal( 2, JSInterop.Invocations["addSubscription"].Count );

        DocumentObserverJsSubscription keyboardSubscription = Assert.IsType<DocumentObserverJsSubscription>(
            JSInterop.Invocations["addSubscription"][0].Arguments[0] );

        Assert.Equal( "keydown", Assert.Single( keyboardSubscription.EventNames ) );
        Assert.Equal( new[] { "ArrowDown", "F4" }, keyboardSubscription.KeysFilter );
        Assert.True( keyboardSubscription.PreventDefault );
        Assert.Equal(
            $"[id=\"{comp.Instance.ElementId}\"][data-open-keys]",
            keyboardSubscription.Selector );

        DocumentObserverJsSubscription outsideSubscription = Assert.IsType<DocumentObserverJsSubscription>(
            JSInterop.Invocations["addSubscription"][1].Arguments[0] );

        Assert.Equal( new[] { "pointerdown", "focusin" }, outsideSubscription.EventNames );
        Assert.Equal(
            $"[id=\"{comp.Instance.PickerContainerId}\"], [id=\"{comp.Instance.CalendarId}\"]",
            outsideSubscription.ExcludeSelector );
    }

    [Fact]
    public async Task FocusingInputOpensCalendarByDefault()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>();

        // test
        await comp.Find( "input" ).FocusAsync( new FocusEventArgs() );

        // validate
        Assert.NotNull( comp.Find( "[role='dialog']" ) );
        Assert.False( comp.Instance.FocusCalendarOnOpen );
    }

    [Fact]
    public async Task ClickOnlyTriggerDoesNotOpenCalendarForFocusOrOpeningKeys()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.OpenTrigger, PickerOpenTrigger.Click ) );
        IElement input = comp.Find( "input" );

        // test
        await input.FocusAsync( new FocusEventArgs() );
        await input.KeyDownAsync( new KeyboardEventArgs { Key = "ArrowDown" } );

        // validate
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );

        // test
        await input.ClickAsync( new MouseEventArgs() );

        // validate
        Assert.NotNull( comp.Find( "[role='dialog']" ) );
    }

    [Fact]
    public async Task FocusOnlyTriggerIgnoresPointerInducedFocusAndClick()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.OpenTrigger, PickerOpenTrigger.Focus ) );
        IElement input = comp.Find( "input" );

        // test
        await input.TriggerEventAsync( "onpointerdown", new PointerEventArgs() );
        await input.FocusAsync( new FocusEventArgs() );
        await input.TriggerEventAsync( "onpointerup", new PointerEventArgs() );
        await input.ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );

        // test
        await input.BlurAsync( new FocusEventArgs() );
        await input.FocusAsync( new FocusEventArgs() );

        // validate
        Assert.NotNull( comp.Find( "[role='dialog']" ) );
    }

    [Fact]
    public async Task OpenKeysOnlyTriggerOpensCalendarWithOpeningKey()
    {
        // setup
        IRenderedComponent<DatePicker<DateTime?>> comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( x => x.OpenTrigger, PickerOpenTrigger.OpenKeys ) );
        IElement input = comp.Find( "input" );

        // test
        await input.FocusAsync( new FocusEventArgs() );

        // validate
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );

        // test
        await input.KeyDownAsync( new KeyboardEventArgs { Key = "ArrowDown" } );

        // validate
        Assert.NotNull( comp.Find( "[role='dialog']" ) );
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
        JSRuntimeInvocation inputMaskInitialization = Assert.Single(
            JSInterop.Invocations["initialize"],
            invocation => invocation.Arguments.Count > 3 && invocation.Arguments[3] is InputMaskJSOptions );
        InputMaskJSOptions options = Assert.IsType<InputMaskJSOptions>( inputMaskInitialization.Arguments[3] );

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
    public async Task EqualRangeDatesAreDisplayedOnce()
    {
        // setup
        DateTime date = new( 2026, 7, 16 );
        IRenderedComponent<DatePicker<DateTime[]>> comp = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.Value, new[] { date } )
            .Add( x => x.SelectionMode, DateInputSelectionMode.Range )
            .Add( x => x.DisplayFormat, "yyyy-MM-dd" ) );

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( "[id$='day-20260716']" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( "[id$='day-20260716']" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( new[] { date }, comp.Instance.Value );
        Assert.Equal( "2026-07-16", comp.Find( "input" ).GetAttribute( "value" ) );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task TodayButtonCommitsRangeOnFirstClick()
    {
        // setup
        DateTime today = DateTime.Today;
        IRenderedComponent<DatePicker<DateTime[]>> comp = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.SelectionMode, DateInputSelectionMode.Range )
            .Add( x => x.ShowTodayButton, true )
            .Add( x => x.DisplayFormat, "yyyy-MM-dd" )
            .Add( x => x.RangeSeparator, " to " ) );

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( ".datepicker-button" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( new[] { today }, comp.Instance.Value );
        Assert.Equal( $"{today:yyyy-MM-dd}", comp.Find( "input" ).GetAttribute( "value" ) );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
    }

    [Fact]
    public async Task TodayButtonReplacesPendingRange()
    {
        // setup
        DateTime today = DateTime.Today;
        DateTime pendingStart = today.AddDays( -1 );
        IRenderedComponent<DatePicker<DateTime[]>> comp = Render<DatePicker<DateTime[]>>( parameters => parameters
            .Add( x => x.SelectionMode, DateInputSelectionMode.Range )
            .Add( x => x.ShowTodayButton, true ) );

        // test
        await comp.Find( "input" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( $"[id$='day-{pendingStart:yyyyMMdd}']" ).ClickAsync( new MouseEventArgs() );
        await comp.Find( ".datepicker-button" ).ClickAsync( new MouseEventArgs() );

        // validate
        Assert.Equal( new[] { today }, comp.Instance.Value );
        Assert.Empty( comp.FindAll( "[role='dialog']" ) );
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