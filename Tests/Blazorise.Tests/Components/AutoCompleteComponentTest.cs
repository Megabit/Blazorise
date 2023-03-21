#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Shared.Models;
using Blazorise.Tests.Helpers;
using Bunit;
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
    public void Focus_ShouldFocus()
    {
        TestFocus<AutocompleteComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]
    public void Clear_ShouldReset()
    {
        TestClear<AutocompleteComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedText );
    }

    [Fact]
    public void InitialSelectedValue_ShouldSet_SelectedText()
    {
        TestInitialSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( "Portugal" )]
    [InlineData( "Antarctica" )]
    [InlineData( "United Kingdom" )]
    [InlineData( "China" )]
    public void SelectValue_ShouldSet( string expectedText )
    {
        TestSelectValue<AutocompleteComponent>( expectedText, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( "MyCustomValue" )]
    public void FreeTypedValue_ShouldSet( string freeTyped )
    {
        TestFreeTypedValue<AutocompleteComponent>( freeTyped, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( true, "Portuga", "Portugal" )]
    [InlineData( true, "Chin", "China" )]
    [InlineData( true, "United King", "United Kingdom" )]
    [InlineData( false, "Portuga", "Portuga" )]
    [InlineData( false, "Chin", "Chin" )]
    [InlineData( false, "United King", "United King" )]
    public void FreeTypedValue_AutoPreSelect_ShouldSet( bool autoPreSelect, string freeTyped, string expectedText )
    {
        TestFreeTypedValue_AutoPreSelect<AutocompleteComponent>( autoPreSelect, freeTyped, expectedText, ( comp ) => comp.Instance.SelectedText );
    }

    [Fact]
    public void AutoPreSelect_True_Should_AutoPreSelectFirstItem()
    {
        TestHasPreselection<AutocompleteComponent>();
    }

    [Fact]
    public void AutoPreSelect_False_ShouldNot_AutoPreSelectFirstItem()
    {
        TestHasNotPreselection<AutocompleteComponent>();
    }

    [Fact]
    public void MinLength_0_ShouldShowOptions_OnFocus()
    {
        TestMinLen0ShowsOptions<AutocompleteComponent>();
    }

    [Fact]
    public void MinLength_BiggerThen0_ShouldNotShowOptions_OnFocus()
    {
        TestMinLenBiggerThen0DoesNotShowOptions<AutocompleteComponent>();
    }

    [Theory]
    [InlineData( "CN", "China" )]
    [InlineData( "PT", "Portugal" )]
    [InlineData( "GB", "United Kingdom" )]
    public void ProgramaticallySetSelectedValue_ShouldSet_SelectedText( string selectedValue, string expectedSelectedText )
    {
        TestProgramaticallySetSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText, selectedValue, expectedSelectedText );
    }
}