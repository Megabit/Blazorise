using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class OnScreenKeyboardProviderComponentTest : BunitContext
{
    public OnScreenKeyboardProviderComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseTextInput();
        JSInterop.AddBlazoriseButton();
        JSInterop.AddBlazoriseClosable();
    }

    [Fact]
    public async Task SpecialCharactersKey_ShouldUseEffectiveActiveState()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.ShowSpecialCharactersKey, true ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );
        await FindButtonByText( comp, "?123" ).ClickAsync();

        comp.WaitForAssertion( () => Assert.Contains( "ABC", comp.Markup ) );

        comp.SetParametersAndRender( parameters => parameters
            .Add( p => p.ShowSpecialCharactersKey, false ) );

        Assert.DoesNotContain( "ABC", comp.Markup );
        Assert.DoesNotContain( "?123", comp.Markup );
    }

    [Fact]
    public async Task ShowSpecialCharactersKey_ShouldUseGlobalOption_WhenParameterIsNotDefined()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var options = Services.GetRequiredService<BlazoriseOptions>();
        options.AccessibilityOptions.OnScreenKeyboard.ShowSpecialCharactersKey = true;

        var comp = Render<OnScreenKeyboardProvider>();

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( "?123", comp.Markup ) );
    }

    [Fact]
    public async Task ShowSpecialCharactersKey_ShouldOverrideGlobalOption_WhenParameterIsDefined()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var options = Services.GetRequiredService<BlazoriseOptions>();
        options.AccessibilityOptions.OnScreenKeyboard.ShowSpecialCharactersKey = true;

        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.ShowSpecialCharactersKey, false ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( "Enter", comp.Markup ) );
        Assert.DoesNotContain( "?123", comp.Markup );
    }

    [Fact]
    public async Task SpecialCharactersRows_ShouldBeCustomizable()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> customRows = new List<IReadOnlyList<OnScreenKeyboardKey>>
        {
            new[] { new OnScreenKeyboardKey( "~" ) },
            new[] { new OnScreenKeyboardKey( OnScreenKeyboardKeyType.SpecialCharacters, "ABC" ) },
        };

        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.ShowSpecialCharactersKey, true )
            .Add( p => p.SpecialCharactersRows, customRows ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );
        await FindButtonByText( comp, "?123" ).ClickAsync();

        comp.WaitForAssertion( () => Assert.Contains( "~", comp.Markup ) );
        Assert.Empty( comp.FindAll( "button" ).Where( button => button.TextContent.Trim() == "!" ) );
    }

    [Fact]
    public async Task LayoutProvider_ShouldCustomizeRows()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> rows = new List<IReadOnlyList<OnScreenKeyboardKey>>
        {
            new[] { new OnScreenKeyboardKey( "custom" ) },
        };

        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.LayoutProvider, _ => rows ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( "custom", comp.Markup ) );
        Assert.DoesNotContain( "q", comp.Markup );
    }

    [Fact]
    public async Task DecimalLayout_ShouldUseContextDecimalSeparator()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>();

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Decimal, decimalSeparator: "," );

        comp.WaitForAssertion( () => Assert.Contains( comp.FindAll( "button" ), button => button.TextContent.Trim() == "," ) );
        Assert.Empty( comp.FindAll( "button" ).Where( button => button.TextContent.Trim() == "." ) );
    }

    [Fact]
    public async Task Preview_ShouldRenderCaret_WhenContextProvidesCaret()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>();

        await keyboardService.Show( new()
        {
            ElementId = "input",
            Layout = OnScreenKeyboardLayout.Decimal,
            GetPreviewValue = () => "1234",
            GetPreviewCaret = () => 2,
        } );

        comp.WaitForAssertion( () => Assert.Contains( "12", comp.Markup ) );
        Assert.Contains( "|", comp.Markup );
        Assert.Contains( "34", comp.Markup );
    }

    [Fact]
    public async Task Shift_ShouldReset_WhenSpecialCharactersAreEnabled()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.ShowSpecialCharactersKey, true ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );
        await FindButtonByText( comp, "Shift" ).ClickAsync();

        comp.WaitForAssertion( () => Assert.Contains( "SHIFT", comp.Markup ) );

        await FindButtonByText( comp, "?123" ).ClickAsync();

        Assert.DoesNotContain( "SHIFT", comp.Markup );
        Assert.Contains( "ABC", comp.Markup );
    }

    [Fact]
    public async Task Enter_ShouldHideKeyboard_WhenHideOnEnterIsEnabled()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>();

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( "Enter", comp.Markup ) );

        await FindButtonByText( comp, "Enter" ).ClickAsync();

        comp.WaitForAssertion( () => Assert.DoesNotContain( "Enter", comp.Markup ) );
        Assert.False( keyboardService.State.Visible );
    }

    [Fact]
    public async Task Provider_ShouldAllowInheritedBaseComponentOverrides()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.Background, Background.Dark )
            .Add( p => p.Padding, Padding.Is4 )
            .Add( p => p.Shadow, Shadow.None ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( "bg-dark", comp.Markup ) );
        Assert.Contains( "p-4", comp.Markup );
        Assert.DoesNotContain( "shadow", comp.Markup );
    }

    [Fact]
    public async Task Keys_ShouldRenderAccessibleLabels()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.ShowSpecialCharactersKey, true ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        var specialCharactersKey = FindButtonByText( comp, "?123" );

        Assert.Equal( "Special characters", specialCharactersKey.GetAttribute( "aria-label" ) );
    }

    [Fact]
    public async Task Provider_ShouldRegisterAsClosableLight_WhenVisible()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();

        var comp = Render<OnScreenKeyboardProvider>();

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => JSInterop.VerifyInvoke( "registerClosableLightComponent" ) );
    }

    [Fact]
    public async Task Provider_ShouldAllowZIndexOverride()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<OnScreenKeyboardProvider>( parameters => parameters
            .Add( p => p.ZIndex, 12345 ) );

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( "z-index:12345", comp.Markup ) );
    }

    [Fact]
    public async Task Provider_ShouldUseStyleProviderZIndex_WhenZIndexIsNotDefined()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var styleProvider = Services.GetRequiredService<IStyleProvider>();
        var comp = Render<OnScreenKeyboardProvider>();

        await ShowKeyboard( keyboardService, OnScreenKeyboardLayout.Text );

        comp.WaitForAssertion( () => Assert.Contains( $"z-index:{styleProvider.DefaultOnScreenKeyboardZIndex}", comp.Markup ) );
    }

    private static Task ShowKeyboard( IOnScreenKeyboardService keyboardService, OnScreenKeyboardLayout layout, string decimalSeparator = null )
    {
        return keyboardService.Show( new()
        {
            ElementId = "input",
            Layout = layout,
            DecimalSeparator = decimalSeparator,
            Enter = () => keyboardService.Hide( "input" ),
        } );
    }

    private static IElement FindButtonByText( IRenderedComponent<OnScreenKeyboardProvider> comp, string text )
    {
        comp.WaitForAssertion( () => Assert.Contains( comp.FindAll( "button" ), button => button.TextContent.Trim() == text ) );

        return comp.FindAll( "button" ).Single( button => button.TextContent.Trim() == text );
    }
}

public class OnScreenKeyboardInputComponentTest : BunitContext
{
    public OnScreenKeyboardInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.OnScreenKeyboard.Enabled = true;
            options.AccessibilityOptions.OnScreenKeyboard.EnterKeyBehavior = OnScreenKeyboardEnterKeyBehavior.Submit;
        } ) );
        JSInterop.AddBlazoriseTextInput();
        JSInterop.AddBlazoriseDatePicker();
        JSInterop.AddBlazoriseTimePicker();
        JSInterop.AddBlazoriseNumericInput();
    }

    [Fact]
    public async Task Enter_ShouldSubmitClosestForm_WhenEnterKeyBehaviorIsSubmit()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<Form>( parameters => parameters
            .Add( p => p.ChildContent, builder =>
            {
                builder.OpenComponent<TextInput>( 0 );
                builder.CloseComponent();
            } ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        JSInterop.VerifyInvoke( "submitClosestForm" );
    }

    [Fact]
    public async Task OnScreenKeyboardParameter_ShouldOverrideGlobalEnabledOption()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.OnScreenKeyboard, false ) );

        await comp.Find( "input" ).FocusInAsync();

        Assert.False( keyboardService.State.Visible );
    }

    [Fact]
    public async Task InsertText_ShouldUseCaretPosition()
    {
        var module = JSInterop.SetupModule( new JSUtilitiesModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.Setup<TextSelection>( "getSelection", _ => true ).SetResult( new() { Start = 1, End = 1 } );

        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var value = "abc";
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "X" );

        Assert.Equal( "aXbc", value );
        JSInterop.VerifyInvoke( "setCaret" );
    }

    [Fact]
    public async Task InsertText_ShouldReplaceSelectedText()
    {
        var module = JSInterop.SetupModule( new JSUtilitiesModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.Setup<TextSelection>( "getSelection", _ => true ).SetResult( new() { Start = 0, End = 4 } );

        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var value = "TEST";
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "d" );

        Assert.Equal( "d", value );
        JSInterop.VerifyInvoke( "setCaret" );
    }

    [Fact]
    public async Task Backspace_ShouldRemoveSelectedText()
    {
        var module = JSInterop.SetupModule( new JSUtilitiesModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.Setup<TextSelection>( "getSelection", _ => true ).SetResult( new() { Start = 1, End = 3 } );

        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var value = "TEST";
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.Backspace();

        Assert.Equal( "TT", value );
        JSInterop.VerifyInvoke( "setCaret" );
    }

    [Fact]
    public async Task DateInput_ShouldKeepKeyboardText_WhenPartialValueCannotParse()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "0" );
        await keyboardService.InsertText( "5" );

        Assert.Equal( "date", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "05/d/yyyy", keyboardService.State.Context.GetPreviewValue() );
        Assert.Null( value );
    }

    [Fact]
    public async Task DateInput_ShouldCommitKeyboardText_WhenValueCanParse()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "05252026" );

        Assert.Equal( "date", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "2026-05-25", keyboardService.State.Context.GetValue() );
        Assert.Equal( "05/25/2026", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 2026, 5, 25 ), value.Value );
    }

    [Fact]
    public async Task DateInput_ShouldAutoPadSegment_WhenNextDigitCannotBelongToCurrentSegment()
    {
        using var cultureScope = new CultureScope( "en-GB" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1252026" );

        Assert.Equal( "date", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "2026-05-12", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12/05/2026", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 2026, 5, 12 ), value.Value );
    }

    [Fact]
    public async Task DateInput_ShouldAcceptSeparatedDateWithSingleDigitSegment()
    {
        using var cultureScope = new CultureScope( "en-GB" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "12/5/2026" );

        Assert.Equal( "2026-05-12", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12/05/2026", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 2026, 5, 12 ), value.Value );
    }

    [Fact]
    public async Task DateInput_ShouldCommitAutoPaddedSegment_WhenKeysArePressedOneByOne()
    {
        using var cultureScope = new CultureScope( "en-GB" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();

        foreach ( char character in "12/5/2026" )
        {
            await keyboardService.InsertText( character.ToString() );
        }

        Assert.Equal( "2026-05-12", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12/05/2026", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 2026, 5, 12 ), value.Value );
    }

    [Fact]
    public async Task DateInput_ShouldExpandTwoDigitYear_WhenSegmentIsCompleted()
    {
        using var cultureScope = new CultureScope( "en-GB" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "12/5/26" );

        Assert.Equal( "2026-05-12", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12/05/2026", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 2026, 5, 12 ), value.Value );
    }

    [Fact]
    public async Task DateTimeInput_ShouldShowTimePlaceholderInPreview_WhenOnlyDateSegmentsAreEntered()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.InputMode, DateInputMode.DateTime )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "05" );

        Assert.Equal( "datetime-local", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "05/dd/yyyy h:mm:ss", keyboardService.State.Context.GetPreviewValue() );
        Assert.Null( value );
    }

    [Fact]
    public async Task DateTimeInput_ShouldNotCommitKeyboardText_UntilRequiredSecondsAreComplete()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.InputMode, DateInputMode.DateTime )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "050555550555" );

        Assert.Equal( "5555-05-05T05:55", keyboardService.State.Context.GetValue() );
        Assert.Equal( "05/05/5555 05:55:ss", keyboardService.State.Context.GetPreviewValue() );
        Assert.Null( value );

        await keyboardService.InsertText( "55" );

        Assert.Equal( "5555-05-05T05:55:55", keyboardService.State.Context.GetValue() );
        Assert.Equal( "05/05/5555 05:55:55", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 5555, 5, 5, 5, 55, 55 ), value.Value );
    }

    [Fact]
    public async Task DateTimeInputEnter_ShouldCompleteMissingSecondsAndCommitValue()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.InputMode, DateInputMode.DateTime )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "050555550555" );
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.Equal( "5555-05-05T05:55:00", keyboardService.State.Context.GetValue() );
        Assert.Equal( "05/05/5555 05:55:00", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 5555, 5, 5, 5, 55, 0 ), value.Value );
    }

    [Fact]
    public async Task DateTimeInputEnter_ShouldCompleteMissingTimeAndCommitValue()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.InputMode, DateInputMode.DateTime )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "05055555" );
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.Equal( "5555-05-05T00:00:00", keyboardService.State.Context.GetValue() );
        Assert.Equal( "05/05/5555 00:00:00", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 5555, 5, 5, 0, 0, 0 ), value.Value );
    }

    [Fact]
    public async Task DateTimeInput_ShouldIgnoreExtraDigits_WhenAllSegmentsAreComplete()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.InputMode, DateInputMode.DateTime )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "05055555055555" );
        await keyboardService.InsertText( "5" );

        Assert.Equal( "5555-05-05T05:55:55", keyboardService.State.Context.GetValue() );
        Assert.Equal( "05/05/5555 05:55:55", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new DateTime( 5555, 5, 5, 5, 55, 55 ), value.Value );
    }

    [Fact]
    public async Task DateTimeInput_ShouldStartNewComposition_WhenTypingAfterCommittedValue()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        DateTime? value = null;
        var comp = Render<DateInput<DateTime?>>( parameters => parameters
            .Add( p => p.InputMode, DateInputMode.DateTime )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "05055555055555" );
        await keyboardService.InsertText( "1" );

        Assert.Equal( "1", keyboardService.State.Context.GetValue() );
        Assert.Equal( "1/dd/yyyy h:mm:ss", keyboardService.State.Context.GetPreviewValue() );
        Assert.Equal( new DateTime( 5555, 5, 5, 5, 55, 55 ), value.Value );
    }

    [Fact]
    public async Task TimeInput_ShouldKeepKeyboardText_WhenPartialValueCannotParse()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1" );
        await keyboardService.InsertText( "2" );

        Assert.Equal( "time", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "12", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12:mm", keyboardService.State.Context.GetPreviewValue() );
        Assert.Null( value );
    }

    [Fact]
    public async Task TimeInput_ShouldCommitKeyboardText_WhenValueCanParse()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1234" );

        Assert.Equal( "time", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "12:34", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12:34", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new TimeSpan( 12, 34, 0 ), value.Value );
    }

    [Fact]
    public async Task TimeInput_ShouldAcceptSeparatedTimeWithSingleDigitSegment()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "9:5" );

        Assert.Equal( "time", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "09:05", keyboardService.State.Context.GetValue() );
        Assert.Equal( "09:05", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new TimeSpan( 9, 5, 0 ), value.Value );
    }

    [Fact]
    public async Task TimeInput_ShouldNotCommitKeyboardText_UntilRequiredSecondsAreComplete()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Step, 1 )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1234" );

        Assert.Equal( "12:34", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12:34:ss", keyboardService.State.Context.GetPreviewValue() );
        Assert.Null( value );

        await keyboardService.InsertText( "56" );

        Assert.Equal( "12:34:56", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12:34:56", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new TimeSpan( 12, 34, 56 ), value.Value );
    }

    [Fact]
    public async Task TimeInputEnter_ShouldCompleteMissingSecondsAndCommitValue()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Step, 1 )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1234" );
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.Equal( "12:34:00", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12:34:00", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new TimeSpan( 12, 34, 0 ), value.Value );
    }

    [Fact]
    public async Task TimeInput_ShouldIgnoreExtraDigits_WhenAllSegmentsAreComplete()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "125555" );
        await keyboardService.InsertText( "5" );

        Assert.Equal( "12:55:55", keyboardService.State.Context.GetValue() );
        Assert.Equal( "12:55:55", keyboardService.State.Context.GetPreviewValue() );
        Assert.True( value.HasValue );
        Assert.Equal( new TimeSpan( 12, 55, 55 ), value.Value );
    }

    [Fact]
    public async Task TimeInput_ShouldStartNewComposition_WhenTypingAfterCommittedValue()
    {
        using var cultureScope = new CultureScope( "en-US" );
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        TimeSpan? value = null;
        var comp = Render<TimeInput<TimeSpan?>>( parameters => parameters
            .Add( p => p.Step, 1 )
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "125555" );
        await keyboardService.InsertText( "1" );

        Assert.Equal( "1", keyboardService.State.Context.GetValue() );
        Assert.Equal( "1:mm:ss", keyboardService.State.Context.GetPreviewValue() );
        Assert.Equal( new TimeSpan( 12, 55, 55 ), value.Value );
    }

    [Fact]
    public async Task DatePicker_ShouldKeepKeyboardText_WhenPartialValueCannotParse()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<DatePicker<DateTime?>>( parameters => parameters
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Instance.OnFocusInHandler( new FocusEventArgs() );
        await keyboardService.InsertText( "2" );
        await keyboardService.InsertText( "0" );

        Assert.Equal( "20", keyboardService.State.Context.GetValue() );
        JSInterop.VerifyInvoke( "updateTextValue" );
    }

    [Fact]
    public async Task TimePicker_ShouldKeepKeyboardText_WhenPartialValueCannotParse()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<TimePicker<TimeSpan?>>( parameters => parameters
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1" );
        await keyboardService.InsertText( "2" );

        Assert.Equal( "12", keyboardService.State.Context.GetValue() );
        JSInterop.VerifyInvoke( "updateTextValue" );
    }

    [Fact]
    public async Task NumericPicker_ShouldUpdateVisiblePickerValue()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<NumericPicker<decimal?>>( parameters => parameters
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "1" );
        await keyboardService.InsertText( "2" );

        Assert.Equal( "12", keyboardService.State.Context.GetValue() );
        JSInterop.VerifyInvoke( "updateValue" );
    }

    [Fact]
    public async Task NumericInput_ShouldKeepDecimalSeparatorInComposition_UntilValueCanParse()
    {
        var module = JSInterop.SetupModule( new JSUtilitiesModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.Setup<int>( "getCaret", _ => true ).SetResult( -1 );

        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        decimal? value = null;
        var comp = Render<NumericInput<decimal?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "123" );

        Assert.Equal( "number", comp.Find( "input" ).GetAttribute( "type" ) );
        Assert.Equal( "123", keyboardService.State.Context.GetValue() );
        Assert.Equal( "123", keyboardService.State.Context.GetPreviewValue() );
        Assert.Equal( 3, keyboardService.State.Context.GetPreviewCaret() );
        Assert.True( value.HasValue );
        Assert.Equal( 123m, value.Value );

        await keyboardService.InsertText( "." );

        Assert.Equal( "123.", keyboardService.State.Context.GetValue() );
        Assert.Equal( "123.", keyboardService.State.Context.GetPreviewValue() );
        Assert.Equal( 4, keyboardService.State.Context.GetPreviewCaret() );
        Assert.True( value.HasValue );
        Assert.Equal( 123m, value.Value );

        await keyboardService.InsertText( "456" );

        Assert.Equal( "123.456", keyboardService.State.Context.GetValue() );
        Assert.Equal( "123.456", keyboardService.State.Context.GetPreviewValue() );
        Assert.Equal( 7, keyboardService.State.Context.GetPreviewCaret() );
        Assert.True( value.HasValue );
        Assert.Equal( 123.456m, value.Value );
    }

    [Fact]
    public async Task NumericInput_ShouldSetKeyboardDecimalSeparatorFromCulture()
    {
        var module = JSInterop.SetupModule( new JSUtilitiesModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.Setup<int>( "getCaret", _ => true ).SetResult( -1 );

        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<NumericInput<decimal?>>( parameters => parameters
            .Add( p => p.Culture, "hr-HR" )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();

        Assert.Equal( ",", keyboardService.State.Context.DecimalSeparator );
    }

    [Fact]
    public async Task NumericInput_ShouldInsertKeyboardTextAtCaret_WhenBrowserExposesCaret()
    {
        var module = JSInterop.SetupModule( new JSUtilitiesModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.Setup<int>( "getCaret", _ => true ).SetResult( 1 );

        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        int? value = 123;
        var comp = Render<NumericInput<int?>>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.InsertText( "9" );

        Assert.Equal( "1923", keyboardService.State.Context.GetValue() );
        Assert.Equal( "1923", keyboardService.State.Context.GetPreviewValue() );
        Assert.Equal( 2, keyboardService.State.Context.GetPreviewCaret() );
        Assert.True( value.HasValue );
        Assert.Equal( 1923, value.Value );
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo currentCulture;
        private readonly CultureInfo currentUICulture;

        public CultureScope( string cultureName )
        {
            currentCulture = CultureInfo.CurrentCulture;
            currentUICulture = CultureInfo.CurrentUICulture;

            var culture = CultureInfo.GetCultureInfo( cultureName );

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;
        }
    }
}

public class OnScreenKeyboardMultilineInputComponentTest : BunitContext
{
    public OnScreenKeyboardMultilineInputComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        Services.AddSingleton( serviceProvider => new BlazoriseOptions( serviceProvider, options =>
        {
            options.AccessibilityOptions.OnScreenKeyboard.Enabled = true;
        } ) );
        JSInterop.AddBlazoriseTextInput();
        JSInterop.AddBlazoriseMemoInput();
        JSInterop.AddBlazoriseButton();
    }

    [Fact]
    public async Task MemoInputEnter_ShouldInsertNewLine_ByDefault()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var value = "abc";
        var comp = Render<MemoInput>( parameters => parameters
            .Add( p => p.Value, value )
            .Add( p => p.ValueChanged, changedValue => value = changedValue )
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "textarea" ).FocusInAsync();
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.Equal( $"abc{Environment.NewLine}", value );
    }

    [Fact]
    public async Task TextInputEnter_ShouldClickValidationSubmitButton_ByDefault()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var submitted = false;
        var comp = Render<Validations>( parameters => parameters
            .Add( p => p.ChildContent, builder =>
            {
                builder.OpenComponent<TextInput>( 0 );
                builder.AddAttribute( 1, nameof( TextInput.OnScreenKeyboard ), true );
                builder.CloseComponent();

                builder.OpenComponent<Button>( 2 );
                builder.AddAttribute( 3, nameof( Button.Type ), ButtonType.Submit );
                builder.AddAttribute( 4, nameof( Button.Display ), Display.None );
                builder.AddAttribute( 5, nameof( Button.Clicked ), EventCallback.Factory.Create<MouseEventArgs>( this, _ => submitted = true ) );
                builder.CloseComponent();
            } ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.True( submitted );
        Assert.True( keyboardService.State.Visible );
        JSInterop.VerifyInvoke( "focus" );
    }

    [Fact]
    public async Task Blur_ShouldKeepKeyboardVisible_WhenKeyboardInteractionSuppressesBlur()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<TextInput>( parameters => parameters
            .Add( p => p.OnScreenKeyboard, true ) );

        await comp.Find( "input" ).FocusInAsync();

        Assert.True( keyboardService.State.Visible );

        keyboardService.SuppressHideOnBlur();

        await comp.Find( "input" ).BlurAsync( new FocusEventArgs() );

        Assert.True( keyboardService.State.Visible );
    }

    [Fact]
    public async Task TextInputEnter_ShouldDispatchKeyDown_WhenEnterKeyBehaviorIsCascaded()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var comp = Render<CascadingValue<OnScreenKeyboardEnterKeyBehavior?>>( parameters => parameters
            .Add( p => p.Name, "OnScreenKeyboardEnterKeyBehaviorOverride" )
            .Add( p => p.Value, (OnScreenKeyboardEnterKeyBehavior?)OnScreenKeyboardEnterKeyBehavior.KeyDown )
            .Add( p => p.ChildContent, builder =>
            {
                builder.OpenComponent<TextInput>( 0 );
                builder.AddAttribute( 1, nameof( TextInput.OnScreenKeyboard ), true );
                builder.CloseComponent();
            } ) );

        await comp.Find( "input" ).FocusInAsync();
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        JSInterop.VerifyInvoke( "dispatchKeyboardEvent" );
    }

    [Fact]
    public async Task MemoInputEnter_ShouldClickValidationPrimaryButton_WhenEnterKeyBehaviorIsSubmit()
    {
        var keyboardService = Services.GetRequiredService<IOnScreenKeyboardService>();
        var submitted = false;
        var comp = Render<Validations>( parameters => parameters
            .Add( p => p.ChildContent, builder =>
            {
                builder.OpenComponent<MemoInput>( 0 );
                builder.AddAttribute( 1, nameof( MemoInput.OnScreenKeyboard ), true );
                builder.AddAttribute( 2, nameof( MemoInput.OnScreenKeyboardEnterKeyBehavior ), OnScreenKeyboardEnterKeyBehavior.Submit );
                builder.CloseComponent();

                builder.OpenComponent<Button>( 3 );
                builder.AddAttribute( 4, nameof( Button.Color ), Color.Primary );
                builder.AddAttribute( 5, nameof( Button.Clicked ), EventCallback.Factory.Create<MouseEventArgs>( this, _ => submitted = true ) );
                builder.AddAttribute( 6, nameof( Button.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Submit" ) ) );
                builder.CloseComponent();
            } ) );

        await comp.Find( "textarea" ).FocusInAsync();
        await keyboardService.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.True( submitted );
    }
}

public class OnScreenKeyboardServiceTest
{
    [Fact]
    public void SuppressHideOnBlur_ShouldTemporarilyIgnoreBlur()
    {
        var service = new OnScreenKeyboardService();

        service.SuppressHideOnBlur();

        Assert.True( service.ShouldIgnoreBlur );
    }

    [Fact]
    public async Task PressKey_ShouldInsertTextAndHandleCommands()
    {
        var service = new OnScreenKeyboardService();
        var value = string.Empty;
        var entered = false;

        await service.Show( new()
        {
            ElementId = "input",
            GetValue = () => value,
            SetValue = newValue =>
            {
                value = newValue;
                return Task.CompletedTask;
            },
            Enter = () =>
            {
                entered = true;
                return Task.CompletedTask;
            },
        } );

        await service.PressKey( new( "a" ) );
        await service.PressKey( new( OnScreenKeyboardKeyType.Space, "Space" ) );
        await service.PressKey( new( "b" ) );
        await service.PressKey( new( OnScreenKeyboardKeyType.Backspace, "Backspace" ) );
        await service.PressKey( new( OnScreenKeyboardKeyType.Enter, "Enter" ) );

        Assert.Equal( "a ", value );
        Assert.True( entered );

        await service.PressKey( new( OnScreenKeyboardKeyType.Clear, "Clear" ) );

        Assert.Equal( string.Empty, value );
    }
}