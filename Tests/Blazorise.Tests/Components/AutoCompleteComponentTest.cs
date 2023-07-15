#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Shared.Models;
using Blazorise.Tests.Extensions;
using Blazorise.Tests.Helpers;
using Bunit;
using FluentAssertions;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteComponentTest : AutocompleteBaseComponentTest
{
    public AutocompleteComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddTextEdit( this.JSInterop );
        BlazoriseConfig.JSInterop.AddUtilities( this.JSInterop );
        BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
        BlazoriseConfig.JSInterop.AddDropdown( this.JSInterop );
    }

    [Fact]
    public async Task FreeTypingNotFoundTemplate_Should_DisplayDefinedContent_WhenFreeTypingIsTrue()
    {
        var comp = RenderComponent<AutocompleteComponent>(
            p => p.Add( x => x.FreeTyping, true ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "My Very Own Country";

        await Input( autoComplete, input, false );

        comp.WaitForAssertion( () =>
        {
            comp.Markup.Should().Contain( @"Add ""My Very Own Country""" );
        } );
    }

    [Fact]
    public async Task NotfoundContent_Should_DisplayDefinedContent_When_FreeTypingIsFalse()
    {
        var comp = RenderComponent<AutocompleteComponent>(
            p => p.Add( x => x.FreeTyping, false ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "My Very Own Country";

        await Input( autoComplete, input, false );

        comp.WaitForAssertion( () =>
        {
            comp.Markup.Should().Contain( @" Sorry... My Very Own Country was not found! :(" );
        } );
    }

    [Fact]
    public async Task SearchTextChanged_ShouldTrigger_Once()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SearchTextChanged, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.InputAsync( "S" );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SearchKeyDown_ShouldTrigger_Once()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SearchKeyDown, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.KeyDownAsync( new() { Key = "S" } );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SearchBlur_ShouldTrigger_Once()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SearchBlur, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.FocusAsync( new() );
        await autoComplete.BlurAsync( new() );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SearchFocus_ShouldTrigger_Once()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SearchFocus, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.FocusAsync( new() );

        Assert.Equal( 1, changedCount );
    }


    [Fact]
    public async Task Opened_ShouldTrigger_Once()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.Opened, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task Closed_ShouldTrigger_OnAutocompleteClosed_Once()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.Closed, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SelectedValueChanged_ShouldOnlyTrigger_WhenValueHasBeenFound()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) => changedCount++ ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SelectedValueChanged_OnBackspace_ShouldNotTrigger_IfSameValue_OnCommit()
    {
        var changedCount = 0;
        var selectedValue = string.Empty;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) =>
            {
                selectedValue = x;
                changedCount++;
            } ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portuga" );
        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portug" );

        Assert.Equal( 1, changedCount );
        Assert.Equal( "PT", selectedValue );

        //Selects first item in dropdown, shouldn't retrigger ValueChanged
        await autoComplete.KeyDownAsync( new() { Code = "Enter" } );
        Assert.Equal( 1, changedCount );
        Assert.Equal( "PT", selectedValue );
    }

    [Fact]
    public async Task SelectedValueChanged_OnBackspace_ShouldTriggerNull_OnBlur()
    {
        var changedCount = 0;
        var selectedValue = string.Empty;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) =>
            {
                selectedValue = x;
                changedCount++;
            } ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portuga" );
        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portug" );

        Assert.Equal( 1, changedCount );
        Assert.Equal( "PT", selectedValue );

        await autoComplete.BlurAsync( new() );
        Assert.Equal( 2, changedCount );
        Assert.Null( selectedValue );
    }

    [Fact]
    public async Task SelectedValueChanged_OnBackspace_ShouldTriggerNull_IfNoValue_OnCommit()
    {
        var changedCount = 0;
        var selectedValue = string.Empty;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) =>
            {
                selectedValue = x;
                changedCount++;
            } ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portuga" );
        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portug" );
        await autoComplete.InputAsync( "Portugl" );

        Assert.Equal( 1, changedCount );
        Assert.Equal( "PT", selectedValue );

        //Selects first item in dropdown, shouldn't retrigger ValueChanged
        await autoComplete.KeyDownAsync( new() { Code = "Enter" } );
        Assert.Equal( 2, changedCount );
        Assert.Null( selectedValue );
    }

    [Fact]
    public async Task SelectedValueChanged_OnBackspace_ShouldTriggerNull_IfNoValue_OnBlur()
    {
        var changedCount = 0;
        var selectedValue = string.Empty;
        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) =>
            {
                selectedValue = x;
                changedCount++;
            } ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portuga" );
        await autoComplete.KeyDownAsync( new() { Code = "Backspace" } );
        await autoComplete.InputAsync( "Portug" );
        await autoComplete.InputAsync( "Portugl" );

        Assert.Equal( 1, changedCount );
        Assert.Equal( "PT", selectedValue );

        await autoComplete.BlurAsync( new() );
        Assert.Equal( 2, changedCount );
        Assert.Null( selectedValue );
    }

    [Fact]
    public async Task SelectedValueChanged_OnAnyEntry_ShouldOnlyTrigger_OnCommit()
    {
        var changedCount = 0;
        var selectedValue = string.Empty;

        var comp = RenderComponent<AutocompleteComponent>( p =>
            p.Add( x => x.SelectedValueChanged, ( x ) =>
            {
                selectedValue = x;
                changedCount++;
            } ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
        Assert.Equal( "PT", selectedValue );

        await Input( autoComplete, "China", true );

        Assert.Equal( 2, changedCount );
        Assert.Equal( "CN", selectedValue );
    }

    [Fact]
    public async Task SelectedValueChanged_ShouldOnlyTrigger_WhenValueHasBeenFoundAndCommitted()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
        {
            p.Add( x => x.SelectedValueChanged, ( x ) => changedCount++ );
            p.Add( x => x.Countries,
                new List<Country>()
                { new( "1", "test", "test" ), new( "10", "test", "test" ), new( "100", "test", "test" )} );
        } );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "100";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SelectedValueChanged_ShouldOnlyTrigger_IfValueIsAlreadySet_But_ValueHasNotBeenFound()
    {
        var changedCount = 0;
        var selectedValue = "PT";
        var comp = RenderComponent<AutocompleteComponent>( p =>
            {
                p.Add( x => x.SelectedValue, selectedValue );
                p.Add( x => x.SelectedValueChanged, ( x ) => { selectedValue = x; changedCount++; } );
            }

        );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "A Random Value!";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
        Assert.Equal( default, selectedValue );
    }

    [Fact]
    public async Task SelectedTextChanged_ShouldOnlyTrigger_WhenValueHasBeenFound()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
        {
            p.Add( x => x.SelectedTextChanged, ( x ) => changedCount++ );
            p.Add( x => x.FreeTyping, false );
        } );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
    }

    [Fact]
    public async Task SelectedTextChanged_FreeTyping_ShouldOnlyTrigger_OnEveryKeyStroke()
    {
        var changedCount = 0;
        var comp = RenderComponent<AutocompleteComponent>( p =>
        {
            p.Add( x => x.SelectedTextChanged, ( x ) => changedCount++ );
            p.Add( x => x.FreeTyping, true );
        } );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "Portugal";

        await Input( autoComplete, input, true );

        Assert.Equal( 9, changedCount );
    }

    [Fact]
    public async Task SelectedTextChanged_NoFreeTyping_ShouldOnlyTrigger_IfValueIsAlreadySet_But_TextHasNotBeenFound()
    {
        var changedCount = 0;
        var selectedText = "Portugal";
        var comp = RenderComponent<AutocompleteComponent>( p =>
            {
                p.Add( x => x.FreeTyping, false );
                p.Add( x => x.SelectedText, selectedText );
                p.Add( x => x.SelectedTextChanged, ( x ) => { selectedText = x; changedCount++; } );
            }

        );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        var input = "A Random Value!";

        await Input( autoComplete, input, true );

        Assert.Equal( 1, changedCount );
        Assert.Equal( default, selectedText );
    }

    [Fact]
    public Task Focus_ShouldFocus()
    {
        return TestFocus<AutocompleteComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]
    public Task Clear_ShouldReset()
    {
        return TestClear<AutocompleteComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedText );
    }

    [Fact]
    public void InitialSelectedValue_ShouldSet_SelectedText()
    {
        TestInitialSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText );
    }

    [Fact]
    public void InitialSelectedValueAndText_ShouldSet_SelectedValueAndText()
    {
        TestInitialSelectedValueAndText<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedValue, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( "Portugal" )]
    [InlineData( "Antarctica" )]
    [InlineData( "United Kingdom" )]
    [InlineData( "China" )]
    public Task SelectValue_ShouldSet( string expectedText )
    {
        return TestSelectValue<AutocompleteComponent>( expectedText, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( "MyCustomValue" )]
    public async Task FreeTypedValue_ShouldSet( string freeTyped )
    {
        await TestFreeTypedValue<AutocompleteComponent>( freeTyped, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( true, "Portuga", "Portugal" )]
    [InlineData( true, "Chin", "China" )]
    [InlineData( true, "United King", "United Kingdom" )]
    [InlineData( false, "Portuga", "Portuga" )]
    [InlineData( false, "Chin", "Chin" )]
    [InlineData( false, "United King", "United King" )]
    public async Task FreeTypedValue_AutoPreSelect_ShouldSet( bool autoPreSelect, string freeTyped, string expectedText )
    {
        await TestFreeTypedValue_AutoPreSelect<AutocompleteComponent>( autoPreSelect, freeTyped, expectedText, ( comp ) => comp.Instance.SelectedText );
    }

    [Fact]
    public Task AutoPreSelect_True_Should_AutoPreSelectFirstItem()
    {
        return TestHasPreselection<AutocompleteComponent>();
    }

    [Fact]
    public Task AutoPreSelect_False_ShouldNot_AutoPreSelectFirstItem()
    {
        return TestHasNotPreselection<AutocompleteComponent>();
    }

    [Fact]
    public Task MinLength_0_ShouldShowOptions_OnFocus()
    {
        return TestMinLen0ShowsOptions<AutocompleteComponent>();
    }

    [Fact]
    public Task MinLength_BiggerThen0_ShouldNotShowOptions_OnFocus()
    {
        return TestMinLenBiggerThen0DoesNotShowOptions<AutocompleteComponent>();
    }

    [Theory]
    [InlineData( "CN", "China" )]
    [InlineData( "PT", "Portugal" )]
    [InlineData( "GB", "United Kingdom" )]
    public Task ProgramaticallySetSelectedValue_ShouldSet_SelectedText( string selectedValue, string expectedSelectedText )
    {
        return TestProgramaticallySetSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText, selectedValue, expectedSelectedText );
    }
}