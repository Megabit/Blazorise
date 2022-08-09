#region Using directives
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Tests.Components
{
    public class AutoCompleteComponentTest : TestContext
    {
        public AutoCompleteComponentTest()
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
            // setup
            var comp = RenderComponent<AutocompleteComponent>();
            var selectedText = comp.Instance.selectedAutoCompleteText;
            var expectedSelectedText = "China";

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            Assert.Equal( expectedSelectedText, selectedText );
            Assert.Equal( expectedSelectedText, inputText );
        }

        [Fact]
        public void AutoSelectFirstItem_True_Should_AutoPreSelectFirstItem()
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>( parameters => { 
                    parameters.Add( x => x.MinLength, 0 );
                    parameters.Add( x => x.AutoSelectFirstItem, true );
            } );
            
            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var preSelected = comp.Find( ".b-is-autocomplete-suggestion.focus" );

            Assert.NotNull( preSelected );
        }

        [Fact]
        public void AutoSelectFirstItem_False_ShouldNot_AutoPreSelectFirstItem()
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>( parameters => {
                parameters.Add( x => x.MinLength, 0 );
                parameters.Add( x => x.AutoSelectFirstItem, false );
            } );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var preSelected = comp.FindAll( ".b-is-autocomplete-suggestion.focus" );

            Assert.Empty( preSelected );
        }

        [Fact]
        public void MinLength_0_ShouldShowOptions_OnFocus()
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>( parameters =>
                    parameters.Add( x => x.MinLength, 0 ) );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var options = comp.FindAll( ".b-is-autocomplete-suggestion" );

            Assert.NotEmpty( options );
        }

        [Fact]
        public void MinLength_BiggerThen0_ShouldNotShowOptions_OnFocus()
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>( parameters =>
                    parameters.Add( x => x.MinLength, 1 ) );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var options = comp.FindAll( ".b-is-autocomplete-suggestion" );

            Assert.Empty( options );
        }

        [Theory]
        [InlineData( "CN", "China" )]
        [InlineData( "PT", "Portugal" )]
        [InlineData( "GB", "United Kingdom" )]
        public void ProgramaticallySetSelectedValue_ShouldSet_SelectedText( string selectedValue, string expectedSelectedText )
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>(
                 parameters =>
                    parameters.Add( x => x.selectedSearchValue, selectedValue ) );

            var selectedText = comp.Instance.selectedAutoCompleteText;

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            Assert.Equal( expectedSelectedText, selectedText );
            Assert.Equal( expectedSelectedText, inputText );
        }
    }
}
