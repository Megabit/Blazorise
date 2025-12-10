#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteMultipleReadDataComponentTest : AutocompleteMultipleBaseComponentTest
{
    public AutocompleteMultipleReadDataComponentTest()
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
        return TestFocus<AutocompleteMultipleReadDataComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]
    public Task Clear_ShouldReset()
    {
        return TestClear<AutocompleteMultipleReadDataComponent>( async ( comp ) => await comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedTexts?.ToArray() );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, "" )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }, "" )]
    public Task SelectValues_ShouldSet( string[] expectedTexts, string dummy )
    {
        return TestSelectValues<AutocompleteMultipleReadDataComponent>( expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "MyCustomValue", "YetAnotherCustomValue" }, new[] { "Portugal", "Croatia", "MyCustomValue", "YetAnotherCustomValue" } )]
    public Task FreeTypedValue_ShouldSet( string[] startTexts, string[] addTexts, string[] expectedTexts )
    {
        return TestFreeTypedValue<AutocompleteMultipleReadDataComponent>( startTexts, addTexts, expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia" }, new[] { "Portugal" } )]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia", "Portugal" }, new string[0] )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }
        , new[] { "Antarctica", "Argentina", "United Kingdom", "Canada" }
        , new[] { "United Arab Emirates", "Afghanistan", "Angola", "Switzerland", "China", "Portugal", "Croatia" } )]
    public Task RemoveValues_ShouldRemove( string[] startTexts, string[] removeTexts, string[] expectedTexts )
    {
        return TestRemoveValues<AutocompleteMultipleReadDataComponent>( startTexts, removeTexts, expectedTexts );
    }

    [Fact]
    public async Task RemovePreviousSelection_WhenLastReadDataDoesNotContainIt_ShouldKeepRemainingSelection()
    {
        var comp = RenderComponent<AutocompleteMultipleReadDataComponent>( parameters =>
            parameters.Add( p => p.MinLength, 0 ) );

        var autoComplete = comp.Find( ".b-is-autocomplete input" );

        await autoComplete.FocusAsync( new() );
        await autoComplete.InputAsync( "Antarctica" );
        AutocompleteBaseComponentTest.WaitAndClickfirstOption( comp, "Antarctica", true );

        await autoComplete.FocusAsync( new() );
        await autoComplete.InputAsync( "Zambia" );
        AutocompleteBaseComponentTest.WaitAndClickfirstOption( comp, "Zambia", true );

        var badges = comp.FindAll( ".b-is-autocomplete .badge" );
        var badgeToRemove = badges.Single( x => x.TextContent.Replace( "×", "" ) == "Antarctica" );
        var removeButton = badgeToRemove.GetElementsByTagName( "span" )[0];
        await removeButton.ClickAsync( new() );

        comp.WaitForAssertion( () =>
        {
            badges.Refresh();
            Assert.Single( badges );
            Assert.Equal( "Zambia", badges[0].TextContent.Replace( "×", "" ) );
            Assert.Equal( new[] { "Zambia" }, comp.Instance.SelectedTexts );
            Assert.Equal( new[] { "ZM" }, comp.Instance.SelectedValues );
        }, TestExtensions.WaitTime );
    }
}