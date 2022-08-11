#region Using directives
using System;
using BasicTestApp.Client;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class AutocompleteBaseComponentTest : TestContext
    {
        public void TestInitialSelectedValue<TComponent>(Func<IRenderedComponent<TComponent>, string> getSelectedText, string expectedSelectedText) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>( parameters => 
                    parameters.TryAdd( "selectedSearchValue", "CN" )
                );
            var selectedText = getSelectedText( comp );

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            Assert.Equal( expectedSelectedText, selectedText );
            Assert.Equal( expectedSelectedText, inputText );
        }

        public void TestHasPreselection<TComponent>( ) where TComponent : IComponent
        {
            var comp = RenderComponent<AutocompleteComponent>( parameters => {
                parameters.TryAdd( "MinLength", 0 );
                parameters.TryAdd( "AutoPreSelect", true );
            } );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var preSelected = comp.Find( ".b-is-autocomplete-suggestion.focus" );

            Assert.NotNull( preSelected );
        }

        public void TestHasNotPreselection<TComponent>( ) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>( parameters => {
                parameters.TryAdd( "MinLength", 0 );
                parameters.TryAdd( "AutoPreSelect", false );
            } );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var preSelected = comp.FindAll( ".b-is-autocomplete-suggestion.focus" );

            Assert.Empty( preSelected );
        }

        public void TestMinLen0ShowsOptions<TComponent>() where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>( parameters =>
                    parameters.TryAdd( "MinLength", 0 ) );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var options = comp.FindAll( ".b-is-autocomplete-suggestion" );

            Assert.NotEmpty( options );
        }

        public void TestMinLenBiggerThen0DoesNotShowOptions<TComponent>() where TComponent : IComponent
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

        public void TestProgramaticallySetSelectedValue<TComponent>(string selectedValue, string expectedSelectedText) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<AutocompleteComponent>(
                 parameters =>
                    parameters.TryAdd( "selectedSearchValue", selectedValue ) );

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
