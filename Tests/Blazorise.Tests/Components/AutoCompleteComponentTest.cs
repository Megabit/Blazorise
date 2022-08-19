#region Using directives
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components
{
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
        public void InitialSelectedValue_ShouldSet_SelectedText()
        {
            TestInitialSelectedValue<AutocompleteComponent>( ( comp ) => comp.Instance.SelectedText);
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
}
