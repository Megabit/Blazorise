#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class AutocompleteMultipleComponentTest : AutocompleteMultipleBaseComponentTest
{
    public AutocompleteMultipleComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextInput()
            .AddBlazoriseUtilities()
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public async Task TagTemplate_Should_BeAbleTo_Remove()
    {
        var comp = RenderComponent<AutocompleteMultipleComponent>( parameters =>
        {
            parameters.Add( x => x.UseBadgeTemplate, true );
            parameters.Add( x => x.SelectedTexts, new List<string>() { "Portugal", "Croatia" } );
        } );

        var tags = comp.FindAll( ".badge" );
        comp.WaitForAssertion( () =>
        {
            tags.Refresh();
            tags.Count.Should().Be( 2 );
        } );

        await comp.Find( ".badge .badge-close" ).ClickAsync();

        comp.WaitForAssertion( () =>
        {
            tags.Refresh();
            tags.Count.Should().Be( 1 );
        } );
    }

    [Fact]
    public Task TagTemplate_Should_Render()
    {
        var comp = RenderComponent<AutocompleteMultipleComponent>( parameters =>
        {
            parameters.Add( x => x.UseBadgeTemplate, true );
            parameters.Add( x => x.SelectedTexts, new List<string>() { "Portugal", "Croatia" } );
        } );

        var tags = comp.FindAll( ".badge" );
        comp.WaitForAssertion( () =>
        {
            tags.Refresh();
            tags.Count.Should().Be( 2 );
            //The x represents the close button.
            tags.Select( x => x.TextContent ).Should().BeEquivalentTo( new[] { "Portugal×", "Croatia×" } );
        } );

        return Task.CompletedTask;
    }

    [Fact]
    public Task Focus_ShouldFocus()
    {
        return TestFocus<AutocompleteMultipleComponent>( ( comp ) => comp.Instance.AutoCompleteRef.Focus() );
    }

    [Fact]

    public Task Clear_ShouldReset()
    {
        return TestClear<AutocompleteMultipleComponent>( async ( comp ) => await comp.Instance.AutoCompleteRef.Clear(), ( comp ) => comp.Instance.SelectedTexts?.ToArray() );
    }

    [Fact]
    public Task InitialSelectedValues_ShouldSet_SelectedTexts()
    {
        return TestInitialSelectedValues<AutocompleteMultipleComponent>( ( comp ) => comp.Instance.SelectedTexts?.ToArray() );
    }


    [Theory]
    [InlineData( new[] { "PT", "HR" }, new[] { "Portugal", "Croatia" } )]
    [InlineData( new[] { "CN", "GB" }, new[] { "China", "United Kingdom" } )]
    [InlineData( new[] {
            "AQ", "AE", "AF", "CA", "US", "AO", "AR", "CH", "CN", "GB", "PT", "HR" },
        new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "United States", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" } )]
    [InlineData( null, null )]
    public Task ProgramaticallySetSelectedValues_ShouldSet_SelectedTexts( string[] selectedValues, string[] expectedSelectedTexts )
    {
        return TestProgramaticallySetSelectedValues<AutocompleteMultipleComponent>( ( comp ) => comp.Instance.SelectedTexts?.ToArray(), selectedValues, expectedSelectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, "" )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }, "" )]
    public Task SelectValues_ShouldSet( string[] expectedTexts, string dummy )
    {
        return TestSelectValues<AutocompleteMultipleComponent>( expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "MyCustomValue", "YetAnotherCustomValue" }, new[] { "Portugal", "Croatia", "MyCustomValue", "YetAnotherCustomValue" } )]
    public Task FreeTypedValue_ShouldSet( string[] startTexts, string[] addTexts, string[] expectedTexts )
    {
        return TestFreeTypedValue<AutocompleteMultipleComponent>( startTexts, addTexts, expectedTexts );
    }

    [Theory]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia" }, new[] { "Portugal" } )]
    [InlineData( new[] { "Portugal", "Croatia" }, new[] { "Croatia", "Portugal" }, new string[0] )]
    [InlineData( new[] { "Antarctica", "United Arab Emirates", "Afghanistan", "Canada", "Angola", "Argentina", "Switzerland", "China", "United Kingdom", "Portugal", "Croatia" }
        , new[] { "Antarctica", "Argentina", "United Kingdom", "Canada" }
        , new[] { "United Arab Emirates", "Afghanistan", "Angola", "Switzerland", "China", "Portugal", "Croatia" } )]
    public Task RemoveValues_ShouldRemove( string[] startTexts, string[] removeTexts, string[] expectedTexts )
    {
        return TestRemoveValues<AutocompleteMultipleComponent>( startTexts, removeTexts, expectedTexts );
    }
}