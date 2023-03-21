namespace Blazorise.E2E.Tests.Tests.Components.Select;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class SelectTests : BlazorisePageTest
{
    [Test]
    public async Task SelectOptions()
    {
        await SelectTestComponent<SelectNavigationsComponent>();


        var sut = Page.Locator( "#select-value-initialy-selected" );
        var select = sut.GetByRole( AriaRole.Combobox );
        var btnOne = sut.Locator( ".btn-primary" );
        var btnTwo = sut.Locator( ".btn-secondary" );
        var result = sut.Locator( "#select-value-initialy-selected-result" );

        await Expect( select ).ToHaveValueAsync( "two" );
        await Expect( result ).ToHaveTextAsync( "two" );

        await SelectAndExpectValue( select, result, "one" );
        await SelectAndExpectValue( select, result, "two" );
        await SelectAndExpectValue( select, result, "three" );

        await btnOne.ClickAsync();
        await Expect( select ).ToHaveValueAsync( "one" );
        await Expect( result ).ToHaveTextAsync( "one" );

        await btnTwo.ClickAsync();
        await Expect( select ).ToHaveValueAsync( "two" );
        await Expect( result ).ToHaveTextAsync( "two" );

        await btnOne.ClickAsync();
        await Expect( select ).ToHaveValueAsync( "one" );
        await Expect( result ).ToHaveTextAsync( "one" );
    }

    private async Task SelectAndExpectValue( ILocator select, ILocator result, string value )
    {
        await select.SelectOptionAsync( value );
        await Expect( result ).ToHaveTextAsync( value );
        await Expect( select ).ToHaveValueAsync( value );
    }
}