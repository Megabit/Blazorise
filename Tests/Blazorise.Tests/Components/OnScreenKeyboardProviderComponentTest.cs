using System;
using System.Collections.Generic;
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

    private static Task ShowKeyboard( IOnScreenKeyboardService keyboardService, OnScreenKeyboardLayout layout )
    {
        return keyboardService.Show( new()
        {
            ElementId = "input",
            Layout = layout,
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
        module.Setup<int>( "getCaret", _ => true ).SetResult( 1 );

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