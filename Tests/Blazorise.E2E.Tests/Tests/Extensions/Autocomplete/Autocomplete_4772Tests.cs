namespace Blazorise.E2E.Tests.Tests.Extensions.Autocomplete;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class Autocomplete_4772Tests : BlazorisePageTest
{
    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<Autocomplete_4772Component>();
    }

    /// <summary>
    /// Tests whether the autocomplete resets correctly when a value is not found & the component is left, 
    /// when the component is refocused it should show back the options when MinLength is 0.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Test()
    {
        var sut = Page.Locator( ".b-is-autocomplete input[type=search]" );
        var dropdownMenu = Page.Locator( ".dropdown-menu.show" );


        await sut.FillAsync( "teeeeeeeeeeeest" );
        await sut.BlurAsync();

        await Expect( sut ).ToHaveValueAsync( "" );

        await sut.FocusAsync();

        await dropdownMenu.WaitForAsync();
        var optionsCount = await dropdownMenu.Locator( ".dropdown-item" ).CountAsync();
        Assert.AreEqual( 2, optionsCount );
    }
}

