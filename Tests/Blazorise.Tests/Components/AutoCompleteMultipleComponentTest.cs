#region Using directives
using System.Linq;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components
{
    public class AutoCompleteMultipleComponentTest : TestContext
    {
        public AutoCompleteMultipleComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddTextEdit( this.JSInterop );
            BlazoriseConfig.JSInterop.AddUtilities( this.JSInterop );
            BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
            BlazoriseConfig.JSInterop.AddDropdown( this.JSInterop );
        }

        [Fact]
        public void InitialSelectedValues_ShouldSet_SelectedTexts()
        {
            // setup
            var comp = RenderComponent<AutocompleteMultipleComponent>();

            var selectedTexts = comp.Instance.selectedTexts;
            var expectedSelectedTexts = new[] { "Portugal", "Croatia" };

            // test
            var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            for ( int i = 0; i < selectedTexts?.Count; i++ )
            {
                Assert.Single( expectedSelectedTexts, selectedTexts[i] );
            }

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedSelectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
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
            // setup
            var comp = RenderComponent<AutocompleteMultipleComponent>(
                 parameters =>
                    parameters.Add( x => x.selectedValues, selectedValues?.ToList() ) );

            var selectedTexts = comp.Instance.selectedTexts;
            var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            for ( int i = 0; i < selectedTexts?.Count; i++ )
            {
                Assert.Single( expectedSelectedTexts, selectedTexts[i] );
            }

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedSelectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }
    }
}
