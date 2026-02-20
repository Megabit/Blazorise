using System;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Tests.Extensions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class ValidateAutocompleteComponentTest : TestContext
{
    public ValidateAutocompleteComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextInput()
            .AddBlazoriseUtilities()
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public async Task CanValidateAutocompleteSingle_InitiallyBlank()
    {
        var comp = RenderComponent<ValidateAutocompleteComponent>();
        var wrapperSelector = "#validate-autocomplete-single-initially-blank .b-is-autocomplete";

        Assert.Contains( "is-invalid", comp.Find( wrapperSelector ).ClassList );

        await SelectAutocompleteValue( comp, "#validate-autocomplete-single-initially-blank", "Portugal" );

        comp.WaitForAssertion( () =>
            Assert.Contains( "is-valid", comp.Find( wrapperSelector ).ClassList ),
            TestExtensions.WaitTime );

        var input = comp.Find( "#validate-autocomplete-single-initially-blank input" );
        await input.InputAsync( string.Empty );

        comp.WaitForAssertion( () =>
            Assert.Contains( "is-invalid", comp.Find( wrapperSelector ).ClassList ),
            TestExtensions.WaitTime );
    }

    [Fact]
    public async Task CanValidateAutocompleteMultiple_InitiallyBlank()
    {
        var comp = RenderComponent<ValidateAutocompleteComponent>();
        var wrapperSelector = "#validate-autocomplete-multiple-initially-blank .b-is-autocomplete";

        Assert.Contains( "is-invalid", comp.Find( wrapperSelector ).ClassList );

        await SelectAutocompleteValue( comp, "#validate-autocomplete-multiple-initially-blank", "Portugal" );

        comp.WaitForAssertion( () =>
            Assert.Contains( "is-valid", comp.Find( wrapperSelector ).ClassList ),
            TestExtensions.WaitTime );

        var badge = comp.Find( "#validate-autocomplete-multiple-initially-blank .badge" );
        var removeButton = badge.GetElementsByTagName( "span" )[0];
        await removeButton.ClickAsync( new() );

        comp.WaitForAssertion( () =>
            Assert.Contains( "is-invalid", comp.Find( wrapperSelector ).ClassList ),
            TestExtensions.WaitTime );
    }

    private static async Task SelectAutocompleteValue<TComponent>( IRenderedComponent<TComponent> comp, string scopeSelector, string expectedText ) where TComponent : IComponent
    {
        var input = comp.Find( $"{scopeSelector} input" );
        await input.FocusAsync( new() );
        await input.InputAsync( expectedText );

        WaitAndClickFirstOption( comp, scopeSelector, expectedText );
    }

    private static void WaitAndClickFirstOption<TComponent>( IRenderedComponent<TComponent> comp, string scopeSelector, string expectedText ) where TComponent : IComponent
    {
        var iterations = 0;
        while ( true )
        {
            var firstSuggestion = comp.WaitForElement( $"{scopeSelector} .b-is-autocomplete-suggestion", TestExtensions.WaitTime );
            if ( firstSuggestion.TextContent.Contains( expectedText ) )
            {
                firstSuggestion.Click();
                break;
            }

            if ( iterations++ > 10 )
                throw new InvalidOperationException( $"Could not find a valid suggestion for {expectedText}" );

            Thread.Sleep( 100 );
        }
    }
}
