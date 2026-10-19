#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteReadDataComponentTest : AutocompleteBaseComponentTest
{
    public AutocompleteReadDataComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextInput()
            .AddBlazoriseUtilities()
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public Task Focus_ShouldFocus()
    {
        return TestFocus<AutocompleteReadDataComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]
    public Task Clear_ShouldReset()
    {
        return TestClear<AutocompleteReadDataComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedText );
    }


    [Theory]
    [InlineData( "Portugal" )]
    [InlineData( "Antarctica" )]
    [InlineData( "United Kingdom" )]
    [InlineData( "China" )]
    public Task SelectValue_ShouldSet( string expectedText )
    {
        return TestSelectValue<AutocompleteReadDataComponent>( expectedText, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( false, false )]
    [InlineData( false, true )]
    [InlineData( true, false )]
    [InlineData( true, true )]
    public async Task SelectPartialMatch_ShouldPreserveBoundValueAndText( bool freeTyping, bool useKeyboard )
    {
        var comp = Render<AutocompleteReadDataComponent>( parameters => parameters
            .Add( x => x.FreeTyping, freeTyping ) );

        foreach ( var ( search, expectedValue, expectedText ) in new[]
        {
            ( "Port", "PT", "Portugal" ),
            ( "Chin", "CN", "China" ),
        } )
        {
            var input = comp.Find( ".b-is-autocomplete input" );
            await input.FocusAsync( new() );
            await input.InputAsync( search );

            if ( useKeyboard )
            {
                await input.KeyDownAsync( Key.Enter );
            }
            else
            {
                await comp.Find( ".b-is-autocomplete-suggestion" ).ClickAsync( new() );
            }

            await input.BlurAsync( new() );

            comp.WaitForAssertion( () =>
            {
                Assert.Equal( expectedValue, comp.Instance.SelectedValue );
                Assert.Equal( expectedText, comp.Instance.SelectedText );
                Assert.Equal( expectedText, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) );
            }, TestExtensions.WaitTime );
        }

        var editedInput = comp.Find( ".b-is-autocomplete input" );
        await editedInput.FocusAsync( new() );
        await editedInput.InputAsync( "Chinx" );
        await editedInput.BlurAsync( new() );

        comp.WaitForAssertion( () =>
        {
            Assert.Null( comp.Instance.SelectedValue );
            Assert.Equal( freeTyping ? "Chinx" : null, comp.Instance.SelectedText );
            Assert.Equal( freeTyping ? "Chinx" : string.Empty, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) );
        }, TestExtensions.WaitTime );
    }

    [Theory]
    [InlineData( "MyCustomValue" )]
    public Task FreeTypedValue_ShouldSet( string freeTyped )
    {
        return TestFreeTypedValue<AutocompleteReadDataComponent>( freeTyped, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( true, "Portuga", "Portugal" )]
    [InlineData( true, "Chin", "China" )]
    [InlineData( true, "United King", "United Kingdom" )]
    [InlineData( false, "Portuga", "Portuga" )]
    [InlineData( false, "Chin", "Chin" )]
    [InlineData( false, "United King", "United King" )]
    public Task FreeTypedValue_AutoPreSelect_ShouldSet( bool autoPreSelect, string freeTyped, string expectedText )
    {
        return TestFreeTypedValue_AutoPreSelect<AutocompleteReadDataComponent>( autoPreSelect, freeTyped, expectedText, ( comp ) => comp.Instance.SelectedText );
    }


    [Fact]
    public Task AutoPreSelect_True_Should_AutoPreSelectFirstItem()
    {
        return TestHasPreselection<AutocompleteReadDataComponent>();
    }

    [Fact]
    public Task AutoPreSelect_False_ShouldNot_AutoPreSelectFirstItem()
    {
        return TestHasNotPreselection<AutocompleteReadDataComponent>();
    }

    [Fact]
    public Task MinSearchLength_0_ShouldShowOptions_OnFocus()
    {
        return TestMinSearchLength0ShowsOptions<AutocompleteReadDataComponent>();
    }

    [Fact]
    public Task MinSearchLength_BiggerThen0_ShouldNotShowOptions_OnFocus()
    {
        return TestMinSearchLengthBiggerThen0DoesNotShowOptions<AutocompleteReadDataComponent>();
    }

    [Fact]
    public async Task MinSearchLength_0_WithCustomTextField_ShouldShowOptionsAfterSelection_OnFocus()
    {
        var comp = Render<AutocompleteCustomTextReadDataComponent>();

        var autoComplete = comp.Find( ".b-is-autocomplete input" );

        await autoComplete.FocusAsync( new() );
        await autoComplete.InputAsync( "Portugal" );

        WaitAndClickfirstOption( comp, "PT - Portugal", true );

        autoComplete = comp.Find( ".b-is-autocomplete input" );
        await autoComplete.BlurAsync( new() );
        await autoComplete.FocusAsync( new() );

        comp.WaitForAssertion( () =>
        {
            Assert.Equal( string.Empty, comp.Instance.LastReadDataSearchValue );
            Assert.NotEmpty( comp.FindAll( ".b-is-autocomplete-suggestion" ) );
            Assert.DoesNotContain( "Sorry...", comp.Markup );
        }, TestExtensions.WaitTime );
    }
}