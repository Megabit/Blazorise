#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BasicTestApp.Client;
using Blazorise.Tests.Extensions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class AutocompleteBaseComponentTest : TestContext
    {
        public void TestInitialSelectedValue<TComponent>( Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>( parameters =>
                    parameters.TryAdd( "SelectedValue", "CN" )
                );

            var selectedText = getSelectedText( comp );
            string expectedSelectedText = "China";

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            Assert.Equal( expectedSelectedText, selectedText );
            Assert.Equal( expectedSelectedText, inputText );
        }

        public void TestHasPreselection<TComponent>() where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>( parameters =>
            {
                parameters.TryAdd( "MinLength", 1 );
                parameters.TryAdd( "AutoPreSelect", true );
            } );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.KeyDown( "A" );
            autoComplete.Input( "A" );
            autoComplete.Focus();

            comp.WaitForAssertion( () => comp.Find( ".b-is-autocomplete-suggestion.focus" ) );
        }

        public void TestHasNotPreselection<TComponent>() where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>( parameters =>
            {
                parameters.TryAdd( "MinLength", 1 );
                parameters.TryAdd( "AutoPreSelect", false );
            } );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.KeyDown( "A" );
            autoComplete.Input( "A" );
            autoComplete.Focus();

            var preSelected = comp.FindAll( ".b-is-autocomplete-suggestion.focus" );

            Assert.Empty( preSelected );
        }

        public void TestMinLen0ShowsOptions<TComponent>() where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>( parameters =>
                    parameters.TryAdd( "MinLength", 0 ) );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            comp.WaitForAssertion( () => Assert.NotEmpty( comp.FindAll( ".b-is-autocomplete-suggestion" ) ) );
        }

        public void TestMinLenBiggerThen0DoesNotShowOptions<TComponent>() where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>( parameters =>
                    parameters.TryAdd( "MinLength", 1 ) );

            // test
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.Input( "" );
            autoComplete.Focus();

            var options = comp.FindAll( ".b-is-autocomplete-suggestion" );

            Assert.Empty( options );
        }

        public void TestProgramaticallySetSelectedValue<TComponent>( Func<IRenderedComponent<TComponent>, string> getSelectedText, string selectedValue, string expectedSelectedText ) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>(
                 parameters =>
                    parameters.TryAdd( "SelectedValue", selectedValue ) );

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
    }

    public class AutocompleteMultipleBaseComponentTest : TestContext
    {
        public void TestInitialSelectedValues<TComponent>( Func<IRenderedComponent<TComponent>, string[]> getSelectedTexts ) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>( parameters =>
                    parameters.TryAdd( "SelectedValues", new List<string> { "PT", "HR" } )
                );

            var selectedTexts = getSelectedTexts( comp );
            var expectedSelectedTexts = new[] { "Portugal", "Croatia" };

            // test
            var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            this.JSInterop.VerifyInvoke( "registerClosableLightComponent", 1 );

            for ( int i = 0; i < selectedTexts?.Length; i++ )
            {
                Assert.Single( expectedSelectedTexts, selectedTexts[i] );
            }

            if ( expectedSelectedTexts is not null && expectedSelectedTexts.Length > 0 )
                Assert.NotEmpty( badges );

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedSelectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }

        public void TestProgramaticallySetSelectedValues<TComponent>( Func<IRenderedComponent<TComponent>, string[]> getSelectedTexts, string[] selectedValues, string[] expectedSelectedTexts ) where TComponent : IComponent
        {
            // setup
            var comp = RenderComponent<TComponent>(
                 parameters =>
                    parameters.TryAdd( "SelectedValues", selectedValues?.ToList() ) );

            var selectedTexts = getSelectedTexts( comp );
            var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            // test
            var input = comp.Find( ".b-is-autocomplete input" );
            var inputText = input.GetAttribute( "value" );

            // validate
            // validate Dropdown initialize / textfield initialize
            this.JSInterop.VerifyInvoke( "initialize", 2 );
            this.JSInterop.VerifyInvoke( "registerClosableLightComponent", 1 );
            for ( int i = 0; i < selectedTexts?.Length; i++ )
            {
                Assert.Single( expectedSelectedTexts, selectedTexts[i] );
            }

            if ( expectedSelectedTexts is not null && expectedSelectedTexts.Length > 0 )
                Assert.NotEmpty( badges );

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedSelectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }

        public void TestSelectValues<TComponent>( string[] expectedTexts ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>(
                     parameters =>
                        parameters.TryAdd( "SelectedValues", (string[])null ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            foreach ( var expectedText in expectedTexts )
            {
                autoComplete.Focus();
                autoComplete.Input( expectedText );

                var firstSuggestion = comp.Find( ".b-is-autocomplete-suggestion" );
                firstSuggestion.MouseUp();
            }

            var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            if ( expectedTexts is not null && expectedTexts.Length > 0 )
                Assert.NotEmpty( badges );

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }

        public void TestFreeTypedValue<TComponent>( string[] startTexts, string[] addTexts, string[] expectedTexts ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>(
                parameters =>
                     parameters.TryAdd( "SelectedTexts", startTexts.ToList() ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            foreach ( var addText in addTexts )
            {
                autoComplete.Focus();
                autoComplete.Input( addText );
                autoComplete.KeyDown( Key.Enter );
            }

            var badges = comp.FindAll( ".b-is-autocomplete .badge" );
            if ( expectedTexts is not null && expectedTexts.Length > 0 )
                Assert.NotEmpty( badges );

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }

        public void TestRemoveValues<TComponent>( string[] startTexts, string[] removeTexts, string[] expectedTexts ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>(
parameters =>
                parameters.TryAdd( "SelectedTexts", startTexts.ToList() ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            var badges = comp.FindAll( ".b-is-autocomplete .badge" );
            foreach ( var removeText in removeTexts )
            {
                var badgeToRemove = badges.Single( x => x.TextContent.Replace( "×", "" ) == removeText );
                var removeButton = badgeToRemove.GetElementsByTagName( "span" )[0];
                removeButton.Click();
                badges.Refresh();
            }

            badges.Refresh();

            if ( expectedTexts is not null && expectedTexts.Length > 0 )
                Assert.NotEmpty( badges );

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }
    }
}
