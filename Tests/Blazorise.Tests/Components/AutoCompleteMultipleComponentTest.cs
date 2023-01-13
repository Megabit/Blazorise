#region Using directives
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Castle.DynamicProxy.Generators.Emitters;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteMultipleComponentTest : AutocompleteMultipleBaseComponentTest
{
    public AutocompleteMultipleComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddTextEdit( this.JSInterop );
        BlazoriseConfig.JSInterop.AddUtilities( this.JSInterop );
        BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
        BlazoriseConfig.JSInterop.AddDropdown( this.JSInterop );
    }

    [Fact]
    public void Focus_ShouldFocus()
    {
        TestFocus<AutocompleteMultipleComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]
    public Task Clear_ShouldReset()
    {
        return TestClear<AutocompleteMultipleComponent>( async ( comp ) => await comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedTexts?.ToArray() );
    }

    [Fact]
    public void InitialSelectedValues_ShouldSet_SelectedTexts()
    {
        TestInitialSelectedValues<AutocompleteMultipleComponent>( ( comp ) => comp.Instance.SelectedTexts?.ToArray() );
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
        TestProgramaticallySetSelectedValues<AutocompleteMultipleComponent>( ( comp ) => comp.Instance.SelectedTexts?.ToArray(), selectedValues, expectedSelectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, "" )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }, "" )]
    public void SelectValues_ShouldSet( string[] expectedTexts, string dummy )
    {
        TestSelectValues<AutocompleteMultipleComponent>( expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "MyCustomValue", "YetAnotherCustomValue" }, new[] { "Portugal", "Croatia", "MyCustomValue", "YetAnotherCustomValue" } )]
    public void FreeTypedValue_ShouldSet( string[] startTexts, string[] addTexts, string[] expectedTexts )
    {
        TestFreeTypedValue<AutocompleteMultipleComponent>( startTexts, addTexts, expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia" }, new[] { "Portugal" } )]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia", "Portugal" }, new string[0] )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }
        , new[] { "Antarctica", "Argentina", "United Kingdom", "Canada" }
        , new[] { "United Arab Emirates", "Afghanistan", "Angola", "Switzerland", "China", "Portugal", "Croatia" } )]
    public void RemoveValues_ShouldRemove( string[] startTexts, string[] removeTexts, string[] expectedTexts )
    {
        TestRemoveValues<AutocompleteMultipleComponent>( startTexts, removeTexts, expectedTexts );
    }
}