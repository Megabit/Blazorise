#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Tests.Extensions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteBaseComponentTest : TestContext
{

    public async Task TestFocus<TComponent>( Func<IRenderedComponent<TComponent>, Task> focus ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>();

        await comp.InvokeAsync( async () => await focus( comp ) );

        comp.WaitForAssertion( () => this.JSInterop.VerifyInvoke( "focus" ), TestExtensions.WaitTime );
    }

    public async Task TestClear<TComponent>( Func<IRenderedComponent<TComponent>, Task> clear, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>( parameters =>
            parameters.TryAdd( "SelectedValue", "CN" )
        );

        await comp.InvokeAsync( async () => await clear( comp ) );
        comp.Render();

        var input = comp.Find( ".b-is-autocomplete input" );
        var inputText = input.GetAttribute( "value" );

        comp.WaitForAssertion( () => Assert.Empty( inputText ), TestExtensions.WaitTime );
        comp.WaitForAssertion( () => Assert.Null( getSelectedText( comp ) ), TestExtensions.WaitTime );
    }

    public async Task TestFreeTypedValue<TComponent>( string freeTypedValue, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>();

        var autoComplete = comp.Find( ".b-is-autocomplete input" );

        await Input( autoComplete, freeTypedValue, true );

        autoComplete.KeyDown( Key.Enter );

        comp.WaitForAssertion( () => Assert.Equal( freeTypedValue, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) ), TestExtensions.WaitTime );
        comp.WaitForAssertion( () => Assert.Equal( freeTypedValue, getSelectedText( comp ) ), TestExtensions.WaitTime );
    }

    public async Task TestFreeTypedValue_AutoPreSelect<TComponent>( bool autoPreSelect, string freeTypedValue, string expectedValue, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>( parameters =>
            parameters.TryAdd( "AutoPreSelect", autoPreSelect ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );

        await Input( autoComplete, freeTypedValue, false );

        WaitAndEnterFirstOption( comp, expectedValue, false );

        comp.WaitForAssertion( () => Assert.Equal( expectedValue, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) ), TestExtensions.WaitTime );
        comp.WaitForAssertion( () => Assert.Equal( expectedValue, getSelectedText( comp ) ), TestExtensions.WaitTime );
    }

    protected static async Task Input( AngleSharp.Dom.IElement autoComplete, string freeTypedValue, bool confirmKey )
    {
        await autoComplete.FocusAsync( new() );
        string inputValue = string.Empty;
        foreach ( var item in freeTypedValue )
        {
            inputValue += item;
            await autoComplete.KeyDownAsync( new() { Key = item.ToString() } );
            await autoComplete.InputAsync( inputValue );
        }

        if ( confirmKey )
            await autoComplete.KeyDownAsync( new() { Code = "Enter" } );
    }

    public async Task TestSelectValue<TComponent>( string expectedText, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>();

        var autoComplete = comp.Find( ".b-is-autocomplete input" );

        await autoComplete.FocusAsync( new() );
        await autoComplete.InputAsync( expectedText );

        WaitAndClickfirstOption( comp, expectedText, true );

        comp.WaitForAssertion( () => Assert.Equal( expectedText, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) ), TestExtensions.WaitTime );
        comp.WaitForAssertion( () => Assert.Equal( expectedText, getSelectedText( comp ) ), TestExtensions.WaitTime );
    }

    public static void WaitAndClickfirstOption<TComponent>( IRenderedComponent<TComponent> comp, string expectedText, bool throwIfNotFound ) where TComponent : IComponent
    {
        var iterations = 0;
        while ( true )
        {
            if ( iterations > 10 )
            {
                if ( throwIfNotFound )
                    throw new Exception( $"Could not find a valid suggestion for {expectedText}" );
                else
                    break;
            }

            var firstSuggestion = comp.WaitForElement( ".b-is-autocomplete-suggestion", TestExtensions.WaitTime );
            if ( firstSuggestion.TextContent.Contains( expectedText ) )
            {
                firstSuggestion.Click();
                break;
            }
            Thread.Sleep( 100 );
            iterations++;
        }
    }


    public static void WaitAndEnterFirstOption<TComponent>( IRenderedComponent<TComponent> comp, string expectedText, bool throwIfNotFound ) where TComponent : IComponent
    {
        var iterations = 0;
        while ( true )
        {
            if ( iterations > 10 )
            {
                if ( throwIfNotFound )
                    throw new Exception( $"Could not find a valid suggestion for {expectedText}" );
                else
                    break;
            }

            var firstSuggestion = comp.WaitForElement( ".b-is-autocomplete-suggestion", TestExtensions.WaitTime );
            if ( firstSuggestion.TextContent.Contains( expectedText ) )
                break;

            Thread.Sleep( 100 );
            iterations++;
        }
        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        autoComplete.KeyDown( Key.Enter );
    }

    public Task TestInitialSelectedValue<TComponent>( Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>( parameters =>
            parameters.TryAdd( "SelectedValue", "CN" )
        );

        var selectedText = getSelectedText( comp );
        string expectedSelectedText = "China";

        // test
        var input = comp.Find( ".b-is-autocomplete input" );
        var inputText = input.GetAttribute( "value" );

        // validate
        // validate Dropdown initialize / textfield initialize
        this.JSInterop.VerifyInvoke( "initialize", 2 );
        Assert.Equal( expectedSelectedText, selectedText );
        Assert.Equal( expectedSelectedText, inputText );
        return Task.CompletedTask;
    }

    public Task TestInitialSelectedValueAndText<TComponent>( Func<IRenderedComponent<TComponent>, string> getSelectedValue, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>( parameters =>
        {
            parameters.TryAdd( "SelectedValue", "CN" );
            parameters.TryAdd( "SelectedText", "China" );
        }
        );

        var selectedText = getSelectedText( comp );
        string expectedSelectedText = "China";

        var selectedValue = getSelectedValue( comp );
        string expectedSelectedValue = "CN";

        // test
        var input = comp.Find( ".b-is-autocomplete input" );
        var inputText = input.GetAttribute( "value" );

        // validate
        // validate Dropdown initialize / textfield initialize
        this.JSInterop.VerifyInvoke( "initialize", 2 );
        Assert.Equal( expectedSelectedText, selectedText );
        Assert.Equal( expectedSelectedText, inputText );

        Assert.Equal( expectedSelectedValue, selectedValue );
        return Task.CompletedTask;
    }

    public async Task TestHasPreselection<TComponent>() where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>( parameters =>
        {
            parameters.TryAdd( "MinSearchLength", 1 );
            parameters.TryAdd( "AutoPreSelect", true );
        } );

        // test
        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "A" } );
        await autoComplete.InputAsync( "A" );
        await autoComplete.FocusAsync( new() );

        comp.WaitForAssertion( () => comp.Find( ".b-is-autocomplete-suggestion.focus" ), TestExtensions.WaitTime );
    }

    public async Task TestHasNotPreselection<TComponent>() where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>( parameters =>
        {
            parameters.TryAdd( "MinSearchLength", 1 );
            parameters.TryAdd( "AutoPreSelect", false );
        } );

        // test
        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "A" } );
        await autoComplete.InputAsync( "A" );
        await autoComplete.FocusAsync( new() );

        var preSelected = comp.FindAll( ".b-is-autocomplete-suggestion.focus" );

        Assert.Empty( preSelected );
    }

    public async Task TestMinSearchLength0ShowsOptions<TComponent>() where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>( parameters =>
            parameters.TryAdd( "MinSearchLength", 0 ) );

        // test
        var autoComplete = comp.WaitForElement( ".b-is-autocomplete input", TestExtensions.WaitTime );
        await autoComplete.FocusAsync( new() );

        comp.WaitForAssertion( () => Assert.NotEmpty( comp.FindAll( ".b-is-autocomplete-suggestion" ) ), TestExtensions.WaitTime );
    }

    public async Task TestMinSearchLengthBiggerThen0DoesNotShowOptions<TComponent>() where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>( parameters =>
            parameters.TryAdd( "MinSearchLength", 1 ) );

        // test
        var autoComplete = comp.WaitForElement( ".b-is-autocomplete input", TestExtensions.WaitTime );
        await autoComplete.FocusAsync( new() );

        var options = comp.FindAll( ".b-is-autocomplete-suggestion" );

        Assert.Empty( options );
    }

    public Task TestProgramaticallySetSelectedValue<TComponent>( Func<IRenderedComponent<TComponent>, string> getSelectedText, string selectedValue, string expectedSelectedText ) where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>(
            parameters =>
                parameters.TryAdd( "SelectedValue", selectedValue ) );

        var selectedText = getSelectedText( comp );

        // test
        var input = comp.Find( ".b-is-autocomplete input" );
        var inputText = input.GetAttribute( "value" );

        // validate
        // validate Dropdown initialize / textfield initialize
        this.JSInterop.VerifyInvoke( "initialize", 2 );
        Assert.Equal( expectedSelectedText, selectedText );
        Assert.Equal( expectedSelectedText, inputText );

        return Task.CompletedTask;
    }
}

public class AutocompleteMultipleBaseComponentTest : TestContext
{
    public async Task TestFocus<TComponent>( Func<IRenderedComponent<TComponent>, Task> focus ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>();

        await comp.InvokeAsync( async () => await focus( comp ) );

        comp.WaitForAssertion( () => this.JSInterop.VerifyInvoke( "focus" ), TestExtensions.WaitTime );
    }

    public async Task TestClear<TComponent>( Func<IRenderedComponent<TComponent>, Task> clear, Func<IRenderedComponent<TComponent>, string[]> getSelectedTexts ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>( parameters =>
        {
            parameters.TryAdd( "SelectedValues", new List<string> { "PT", "HR" } );
            parameters.TryAdd( "MinSearchLength", 0 );
        } );

        await comp.InvokeAsync( async () => await clear( comp ) );

        var input = comp.Find( ".b-is-autocomplete input" );
        var inputText = input.GetAttribute( "value" );

        var badges = comp.FindAll( ".b-is-autocomplete .badge" );

        comp.WaitForAssertion( () => Assert.Empty( inputText ), TestExtensions.WaitTime );
        comp.WaitForAssertion( () => Assert.Empty( badges ), TestExtensions.WaitTime );
        comp.WaitForAssertion( () => Assert.Empty( getSelectedTexts( comp ) ), TestExtensions.WaitTime );
    }

    public Task TestInitialSelectedValues<TComponent>( Func<IRenderedComponent<TComponent>, string[]> getSelectedTexts ) where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>( parameters =>
            parameters.TryAdd( "SelectedValues", new List<string> { "PT", "HR" } )
        );

        var selectedTexts = getSelectedTexts( comp );
        var expectedSelectedTexts = new[] { "Portugal", "Croatia" };

        // test
        var badges = comp.FindAll( ".b-is-autocomplete .badge" );

        // validate
        // validate Dropdown initialize / textfield initialize
        this.JSInterop.VerifyInvoke( "initialize", 2 );
        this.JSInterop.VerifyInvoke( "registerClosableLightComponent", 1 );

        for ( int i = 0; i < selectedTexts?.Length; i++ )
        {
            Assert.Single( expectedSelectedTexts, selectedTexts[i] );
        }

        AssertExpectedTextsWithBadges( comp, expectedSelectedTexts );
        return Task.CompletedTask;
    }

    public Task TestProgramaticallySetSelectedValues<TComponent>( Func<IRenderedComponent<TComponent>, string[]> getSelectedTexts, string[] selectedValues, string[] expectedSelectedTexts ) where TComponent : IComponent
    {
        // setup
        var comp = RenderComponent<TComponent>(
            parameters =>
                parameters.TryAdd( "SelectedValues", selectedValues?.ToList() ) );

        var selectedTexts = getSelectedTexts( comp );
        var badges = comp.FindAll( ".b-is-autocomplete .badge" );

        // test
        var input = comp.Find( ".b-is-autocomplete input" );
        var inputText = input.GetAttribute( "value" );

        // validate
        // validate Dropdown initialize / textfield initialize
        this.JSInterop.VerifyInvoke( "initialize", 2 );
        this.JSInterop.VerifyInvoke( "registerClosableLightComponent", 1 );
        for ( int i = 0; i < selectedTexts?.Length; i++ )
        {
            Assert.Single( expectedSelectedTexts, selectedTexts[i] );
        }

        AssertExpectedTextsWithBadges( comp, expectedSelectedTexts );
        return Task.CompletedTask;
    }

    public async Task TestSelectValues<TComponent>( string[] expectedTexts ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>(
            parameters =>
                parameters.TryAdd( "SelectedValues", (string[])null ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        foreach ( var expectedText in expectedTexts )
        {
            await autoComplete.FocusAsync( new() );
            await autoComplete.InputAsync( expectedText );

            AutocompleteBaseComponentTest.WaitAndClickfirstOption( comp, expectedText, true );
        }

        AssertExpectedTextsWithBadges( comp, expectedTexts );
    }

    public async Task TestFreeTypedValue<TComponent>( string[] startTexts, string[] addTexts, string[] expectedTexts ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>(
            parameters =>
                parameters.TryAdd( "SelectedTexts", startTexts.ToList() ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        foreach ( var addText in addTexts )
        {
            await autoComplete.FocusAsync( new() );
            await autoComplete.InputAsync( addText );
            await autoComplete.KeyDownAsync( Key.Enter );
        }

        AssertExpectedTextsWithBadges( comp, expectedTexts );
    }

    public async Task TestRemoveValues<TComponent>( string[] startTexts, string[] removeTexts, string[] expectedTexts ) where TComponent : IComponent
    {
        var comp = RenderComponent<TComponent>(
            parameters =>
                parameters.TryAdd( "SelectedTexts", startTexts.ToList() ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var badges = comp.FindAll( ".b-is-autocomplete .badge" );
        foreach ( var removeText in removeTexts )
        {
            var badgeToRemove = badges.Single( x => x.TextContent.Replace( "×", "" ) == removeText );
            var removeButton = badgeToRemove.GetElementsByTagName( "span" )[0];
            await removeButton.ClickAsync( new() );
            badges.Refresh();
        }

        AssertExpectedTextsWithBadges( comp, expectedTexts );
    }

    private void AssertExpectedTextsWithBadges<TComponent>( IRenderedComponent<TComponent> comp, string[] expectedTexts ) where TComponent : IComponent
    {
        var badges = comp.FindAll( ".b-is-autocomplete .badge" );

        if ( expectedTexts is not null && expectedTexts.Length > 0 )
            comp.WaitForAssertion( () => { badges.Refresh(); Assert.Equal( expectedTexts.Length, badges.Count ); }, TestExtensions.WaitTime );

        for ( int i = 0; i < badges?.Count; i++ )
        {
            Assert.Single( expectedTexts, badges[i].TextContent.Replace( "×", "" ) );
        }
    }
}