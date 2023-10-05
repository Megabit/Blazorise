namespace Blazorise.E2E.Tests.Tests.Extensions.Autocomplete;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class Autocomplete_5038_Tests : BlazorisePageTest
{

    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<Autocomplete_5038Component>();
    }

    /// <summary>
    /// Makes sure that when FreeTyping is set and a value is not found, the FreeTypingNotFoundtemplate is shown.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Test()
    {
        var sut = Page.Locator( ".b-is-autocomplete input[type=search]" );
        var dropdownMenu = Page.Locator( ".dropdown-menu.show" );

        await sut.ClickAsync();
        await sut.FillAsync( "My Color" );
        await sut.PressAsync( "Enter" );
        await Expect( sut ).ToHaveValueAsync( "My Color" );

        await dropdownMenu.WaitForAsync();
        var option = ( await dropdownMenu.Locator( ".dropdown-item" ).AllAsync() ).Single();

        await Expect( option ).ToHaveTextAsync( @"Add ""My Color""" );
    }
}

