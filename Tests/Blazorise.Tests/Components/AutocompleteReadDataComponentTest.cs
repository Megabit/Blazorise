#region Using directives
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteReadDataComponentTest : AutocompleteBaseComponentTest
{
    public AutocompleteReadDataComponentTest()
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
        TestFocus<AutocompleteReadDataComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]
    public void Clear_ShouldReset()
    {
        TestClear<AutocompleteReadDataComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedText );
    }


    [Theory]
    [InlineData( "Portugal" )]
    [InlineData( "Antarctica" )]
    [InlineData( "United Kingdom" )]
    [InlineData( "China" )]
    public void SelectValue_ShouldSet( string expectedText )
    {
        TestSelectValue<AutocompleteReadDataComponent>( expectedText, ( comp ) => comp.Instance.SelectedText );
    }

    [Theory]
    [InlineData( "MyCustomValue" )]
    public void FreeTypedValue_ShouldSet( string freeTyped )
    {
        TestFreeTypedValue<AutocompleteReadDataComponent>( freeTyped, ( comp ) => comp.Instance.SelectedText );
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
        TestFreeTypedValue_AutoPreSelect<AutocompleteReadDataComponent>( autoPreSelect, freeTyped, expectedText, ( comp ) => comp.Instance.SelectedText );
    }


    [Fact]
    public void AutoPreSelect_True_Should_AutoPreSelectFirstItem()
    {
        TestHasPreselection<AutocompleteReadDataComponent>();
    }

    [Fact]
    public void AutoPreSelect_False_ShouldNot_AutoPreSelectFirstItem()
    {
        TestHasNotPreselection<AutocompleteReadDataComponent>();
    }

    [Fact]
    public void MinLength_0_ShouldShowOptions_OnFocus()
    {
        TestMinLen0ShowsOptions<AutocompleteReadDataComponent>();
    }

    [Fact]
    public void MinLength_BiggerThen0_ShouldNotShowOptions_OnFocus()
    {
        TestMinLenBiggerThen0DoesNotShowOptions<AutocompleteReadDataComponent>();
    }

}