namespace Blazorise.E2E.Tests.Tests.Extensions.Autocomplete;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class Autocomplete_4634_Tests : BlazorisePageTest
{

    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<Autocomplete_4634Component>();
    }

    /// <summary>
    /// When a value is already set, setting a different value programmatically and then revisiting should have updated the Current Search Value and options shown.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Test()
    {
        var sut = Page.Locator( ".b-is-autocomplete input[type=search]" );
        var dropdownMenu = Page.Locator( ".dropdown-menu.show" );
        var btnChangeValue = Page.Locator( "#btnChangeValue" );

        await sut.ClickAsync();
        await sut.FillAsync( "Noah" );
        await sut.PressAsync( "Enter" );
        await Expect( sut ).ToHaveValueAsync( "Noah" );

        await sut.BlurAsync();
        await sut.FocusAsync();
        await dropdownMenu.WaitForAsync();
        var option = ( await dropdownMenu.Locator( ".dropdown-item" ).AllAsync() ).Single();
        await Expect( option ).ToHaveTextAsync( "Noah" );

        await sut.BlurAsync();
        await btnChangeValue.ClickAsync();
        await sut.FocusAsync();
        await dropdownMenu.WaitForAsync();
        option = ( await dropdownMenu.Locator( ".dropdown-item" ).AllAsync() ).Single();
        await Expect( option ).ToHaveTextAsync( "Emma" );
    }
}

