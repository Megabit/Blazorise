#region Using directives
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components
{

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
}
