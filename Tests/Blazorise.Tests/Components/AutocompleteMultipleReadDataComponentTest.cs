#region Using directives
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteMultipleReadDataComponentTest : AutocompleteMultipleBaseComponentTest
{
    public AutocompleteMultipleReadDataComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddTextEdit( this.JSInterop );
        BlazoriseConfig.JSInterop.AddUtilities( this.JSInterop );
        BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
        BlazoriseConfig.JSInterop.AddDropdown( this.JSInterop );
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
}