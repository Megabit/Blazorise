﻿#region Using directives
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
            BlazoriseConfig.JSInterop.AddTextEdit(this.JSInterop);
        }


        [Fact]
        public void InitialSelectedValue_ShouldSet_SelectedText()
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>();
            var selectedText = comp.Instance.selectedAutoCompleteText;
            var expectedSelectedText = "Andorra";

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );
            // validate
            this.JSInterop.VerifyInvoke( "initialize" );
            Assert.Equal( expectedSelectedText, selectedText );
            Assert.Equal( expectedSelectedText, inputText );
        }

        [Theory]
        [InlineData( 2, "Andorra" )]
        [InlineData( 1, "Albania" )]
        [InlineData( 8, "Bosnia & Herzegovina" )]
        public void ProgramaticallySetSelectedValue_ShouldSet_SelectedText( int selectedValue, string expectedSelectedText )
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
            this.JSInterop.VerifyInvoke( "initialize" );
            Assert.Equal( expectedSelectedText, selectedText );
            Assert.Equal( expectedSelectedText, inputText );
        }
    }
}
