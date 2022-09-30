﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BasicTestApp.Client;
using Blazorise.Components;
using Blazorise.Tests.Extensions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class AutocompleteBaseComponentTest : TestContext
    {

        public void TestFocus<TComponent>( Action<IRenderedComponent<TComponent>> focus ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>();

            focus( comp );

            comp.WaitForAssertion( () => this.JSInterop.VerifyInvoke( "focus" ), TestExtensions.WaitTime );
        }

        public void TestClear<TComponent>( Action<IRenderedComponent<TComponent>> clear, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>( parameters =>
                parameters.TryAdd( "SelectedValue", "CN" )
            );

            clear( comp );

            //var validateInputTest = () =>
            //{
            //    var input = comp.Find( ".b-is-autocomplete input" );
            //    var inputText = input.GetAttribute( "value" );
            //    Assert.Empty( inputText );
            //};


            //comp.WaitForAssertion( () => validateInputTest(), TestExtensions.WaitTime );
            comp.WaitForAssertion( () => Assert.Null( getSelectedText( comp ) ), TestExtensions.WaitTime );
        }

        public void TestFreeTypedValue<TComponent>( string freeTypedValue, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>();

            var autoComplete = comp.Find( ".b-is-autocomplete input" );

            autoComplete.Focus();
            autoComplete.Input( freeTypedValue );

            foreach ( var item in freeTypedValue )
            {
                autoComplete.KeyDown( item );
            }

            autoComplete.KeyDown( Key.Enter );

            comp.WaitForAssertion( () => Assert.Equal( freeTypedValue, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) ), TestExtensions.WaitTime );
            comp.WaitForAssertion( () => Assert.Equal( freeTypedValue, getSelectedText( comp ) ), TestExtensions.WaitTime );
        }

        public void TestFreeTypedValue_AutoPreSelect<TComponent>( bool autoPreSelect, string freeTypedValue, string expectedValue, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>( parameters =>
                        parameters.TryAdd( "AutoPreSelect", autoPreSelect ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );

            autoComplete.Focus();
            autoComplete.Input( freeTypedValue );

            foreach ( var item in freeTypedValue )
            {
                autoComplete.KeyDown( item );
            }

            WaitAndEnterFirstOption( comp, expectedValue, false );

            comp.WaitForAssertion( () => Assert.Equal( expectedValue, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) ), TestExtensions.WaitTime );
            comp.WaitForAssertion( () => Assert.Equal( expectedValue, getSelectedText( comp ) ), TestExtensions.WaitTime );
        }


        public void TestSelectValue<TComponent>( string expectedText, Func<IRenderedComponent<TComponent>, string> getSelectedText ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>(
                     parameters =>
                        parameters.TryAdd( "SelectedValue", (string[])null ) );

            var autoComplete = comp.Find( ".b-is-autocomplete input" );

            autoComplete.Focus();
            autoComplete.Input( expectedText );

            WaitAndClickfirstOption( comp, expectedText, true );

            comp.WaitForAssertion( () => Assert.Equal( expectedText, comp.Find( ".b-is-autocomplete input" ).GetAttribute( "value" ) ), TestExtensions.WaitTime );
            comp.WaitForAssertion( () => Assert.Equal( expectedText, getSelectedText( comp ) ), TestExtensions.WaitTime );
        }

        public static void WaitAndClickfirstOption<TComponent>( IRenderedComponent<TComponent> comp, string expectedText, bool throwIfNotFound ) where TComponent : IComponent
        {
            var iterations = 0;
            while ( true )
            {
                if ( iterations > 10 )
                {
                    if ( throwIfNotFound )
                        throw new Exception( $"Could not find a valid suggestion for {expectedText}" );
                    else
                        break;
                }

                var firstSuggestion = comp.WaitForElement( ".b-is-autocomplete-suggestion", TestExtensions.WaitTime );
                if ( firstSuggestion.TextContent.Contains( expectedText ) )
                {
                    firstSuggestion.MouseUp();
                    break;
                }
                Thread.Sleep( 100 );
                iterations++;
            }
        }


        public static void WaitAndEnterFirstOption<TComponent>( IRenderedComponent<TComponent> comp, string expectedText, bool throwIfNotFound ) where TComponent : IComponent
        {
            var iterations = 0;
            while ( true )
            {
                if ( iterations > 10 )
                {
                    if ( throwIfNotFound )
                        throw new Exception( $"Could not find a valid suggestion for {expectedText}" );
                    else
                        break;
                }

                var firstSuggestion = comp.WaitForElement( ".b-is-autocomplete-suggestion", TestExtensions.WaitTime );
                if ( firstSuggestion.TextContent.Contains( expectedText ) )
                    break;

                Thread.Sleep( 100 );
                iterations++;
            }
            var autoComplete = comp.Find( ".b-is-autocomplete input" );
            autoComplete.KeyDown( Key.Enter );
        }

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

            comp.WaitForAssertion( () => comp.Find( ".b-is-autocomplete-suggestion.focus" ), TestExtensions.WaitTime );
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

            comp.WaitForAssertion( () => Assert.NotEmpty( comp.FindAll( ".b-is-autocomplete-suggestion" ) ), TestExtensions.WaitTime );
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
        public void TestFocus<TComponent>( Action<IRenderedComponent<TComponent>> focus ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>();

            focus( comp );

            comp.WaitForAssertion( () => this.JSInterop.VerifyInvoke( "focus" ), TestExtensions.WaitTime );
        }

        public void TestClear<TComponent>( Action<IRenderedComponent<TComponent>> clear, Func<IRenderedComponent<TComponent>, string[]> getSelectedTexts ) where TComponent : IComponent
        {
            var comp = RenderComponent<TComponent>( parameters =>
                parameters.TryAdd( "SelectedValues", new List<string> { "PT", "HR" } )
            );

            clear( comp );

            //var input = comp.Find( ".b-is-autocomplete input" );
            //var inputText = input.GetAttribute( "value" );

            //var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            //comp.WaitForAssertion( () => Assert.Empty( inputText ), TestExtensions.WaitTime );
            //comp.WaitForAssertion( () => Assert.Empty( badges ), TestExtensions.WaitTime );
            comp.WaitForAssertion( () => Assert.Empty( getSelectedTexts( comp ) ), TestExtensions.WaitTime );
        }

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

            AssertExpectedTextsWithBadges( comp, expectedSelectedTexts );
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

            AssertExpectedTextsWithBadges( comp, expectedSelectedTexts );
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

                AutocompleteBaseComponentTest.WaitAndClickfirstOption( comp, expectedText, true );
            }

            AssertExpectedTextsWithBadges( comp, expectedTexts );
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

            AssertExpectedTextsWithBadges( comp, expectedTexts );
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

            AssertExpectedTextsWithBadges( comp, expectedTexts );
        }

        private void AssertExpectedTextsWithBadges<TComponent>( IRenderedComponent<TComponent> comp, string[] expectedTexts ) where TComponent : IComponent
        {
            var badges = comp.FindAll( ".b-is-autocomplete .badge" );

            if ( expectedTexts is not null && expectedTexts.Length > 0 )
                comp.WaitForAssertion( () => { badges.Refresh(); Assert.Equal( expectedTexts.Length, badges.Count ); }, TestExtensions.WaitTime );

            for ( int i = 0; i < badges?.Count; i++ )
            {
                Assert.Single( expectedTexts, badges[i].TextContent.Replace( "×", "" ) );
            }
        }
    }
}
