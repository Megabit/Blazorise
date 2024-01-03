#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteCheckboxComponentTest : AutocompleteMultipleBaseComponentTest
{
    public AutocompleteCheckboxComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextEdit()
            .AddBlazoriseUtilities()
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }


    [Fact]
    public void Suggestions_ShouldShow_Checkboxes()
    {
        var comp = RenderComponent<AutocompleteCheckboxComponent>();

        // test
        var autoComplete = comp.Find( ".b-is-autocomplete input" );
        autoComplete.Input( "A" );
        autoComplete.Focus();

        var suggestions = comp.FindAll( ".b-is-autocomplete-suggestion" );

        Assert.All( suggestions, ( x ) => Assert.True( x.InnerHtml.Contains( "b-is-autocomplete-suggestion-checkbox" ) && x.InnerHtml.Contains( "</i>" ) ) );
    }

    [Fact]
    public void InitialSelectedValues_ShouldSet_SelectedTexts()
    {
        TestInitialSelectedValues<AutocompleteCheckboxComponent>( ( comp ) => comp.Instance.SelectedTexts?.ToArray() );
    }


    [Theory]
    [InlineData( new[] { "PT", "HR" }, new[] { "Portugal", "Croatia" } )]
    [InlineData( new[] { "CN", "GB" }, new[] { "China", "United Kingdom" } )]
    [InlineData( new[] {
            "AQ", "AE", "AF", "CA", "US", "AO", "AR", "CH", "CN", "GB", "PT", "HR" },
        new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "United States", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" } )]
    [InlineData( null, null )]
    public void ProgramaticallySetSelectedValues_ShouldSet_SelectedTexts( string[] selectedValues, string[] expectedSelectedTexts )
    {
        TestProgramaticallySetSelectedValues<AutocompleteCheckboxComponent>( ( comp ) => comp.Instance.SelectedTexts?.ToArray(), selectedValues, expectedSelectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, "" )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }, "" )]
    public Task SelectValues_ShouldSet( string[] expectedTexts, string dummy )
    {
        return TestSelectValues<AutocompleteCheckboxComponent>( expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "MyCustomValue", "YetAnotherCustomValue" }, new[] { "Portugal", "Croatia", "MyCustomValue", "YetAnotherCustomValue" } )]
    public Task FreeTypedValue_ShouldSet( string[] startTexts, string[] addTexts, string[] expectedTexts )
    {
        return TestFreeTypedValue<AutocompleteCheckboxComponent>( startTexts, addTexts, expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia" }, new[] { "Portugal" } )]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia", "Portugal" }, new string[0] )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }
        , new[] { "Antarctica", "Argentina", "United Kingdom", "Canada" }
        , new[] { "United Arab Emirates", "Afghanistan", "Angola", "Switzerland", "China", "Portugal", "Croatia" } )]
    public Task RemoveValues_ShouldRemove( string[] startTexts, string[] removeTexts, string[] expectedTexts )
    {
        return TestRemoveValues<AutocompleteCheckboxComponent>( startTexts, removeTexts, expectedTexts );
    }
}