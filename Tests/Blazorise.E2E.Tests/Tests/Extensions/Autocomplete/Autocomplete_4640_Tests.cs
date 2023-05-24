namespace Blazorise.E2E.Tests.Tests.Extensions.Autocomplete;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class Autocomplete_4640_Tests : BlazorisePageTest
{

    [SetUp]
    public async Task Init()
    {
        await SelectTestComponent<Autocomplete_4640Component>();
    }

    [Test]
    public async Task Test()
    {
        var sut = Page.Locator( ".b-is-autocomplete input[type=search]" );

        await sut.ClickAsync();
        await sut.FillAsync( "Hello" );
        await sut.PressAsync( "Enter" );

        await Expect( sut ).ToHaveValueAsync( "Hello" );
        await sut.BlurAsync();
        await Expect( sut ).ToHaveValueAsync( "Hello" );

        await sut.FocusAsync();
        await sut.PressAsync( "Backspace" );
        await sut.PressAsync( "Enter" );
        await Expect( sut ).ToHaveValueAsync( "Hello" );

        await sut.FocusAsync();
        await sut.PressAsync( "Backspace" );
        await sut.BlurAsync();
        await Expect( sut ).ToHaveValueAsync( "" );
    }
}

