namespace Blazorise.E2E.Tests.Tests.Extensions.Autocomplete;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class Autocomplete_4695Tests : BlazorisePageTest
{
    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<Autocomplete_4695Component>();
    }

    /// <summary>
    /// Tests whether the autocomplete resets correctly when the value is cleared, showing back the options when MinSearchLength is 0.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Test()
    {
        var sut = Page.Locator( ".b-is-autocomplete input[type=search]" );
        var dropdownMenu = Page.Locator( ".dropdown-menu.show" );


        await sut.FillAsync( "Test1" );
        await sut.PressAsync( "Enter" );

        await Expect( sut ).ToHaveValueAsync( "Test1" );

        await sut.BlurAsync();
        await sut.FocusAsync();
        await sut.PressAsync( "Escape" );
        await Expect( sut ).ToHaveTextAsync( "" );

        await dropdownMenu.WaitForAsync();
        var optionsCount = await dropdownMenu.Locator( ".dropdown-item" ).CountAsync();
        Assert.AreEqual( 2, optionsCount );

        await sut.FillAsync( "Test2" );
        await sut.PressAsync( "Enter" );
        await Expect( sut ).ToHaveValueAsync( "Test2" );

        await sut.BlurAsync();
        await sut.FocusAsync();
        await sut.FillAsync( "" );

        await dropdownMenu.WaitForAsync();
        optionsCount = await dropdownMenu.Locator( ".dropdown-item" ).CountAsync();
        Assert.AreEqual( 2, optionsCount );

    }
}

