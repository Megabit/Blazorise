using BasicTestApp.Client;
using Blazorise.E2E.Tests.Infrastructure;

namespace Blazorise.E2E.Tests.Tests.Components.RadioGroup;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class RadioGroupTests : BlazorisePageTest
{
    [Test]
    public async Task CanCheckString_InitiallyChecked()
    {
        await SelectTestComponent<RadioGroupComponent>();

        var sut = Page.Locator( "#radiogroup-event-initially-selected" );
        var radioRed = sut.Locator( ".radioR" );
        var radioGreen = sut.Locator( ".radioG" );
        var radioBlue = sut.Locator( ".radioB" );
        var result = sut.Locator( "#radiogroup-event-initially-selected-result" );

        //TODO : FAILING; NO CHECKED Attribute
        //Assert.NotNull( await radioGreen.GetAttributeAsync( "checked" ) );
        await Expect( result ).ToHaveTextAsync( "green" );

        Assert.Null( await radioRed.GetAttributeAsync( "checked" ) );
        Assert.Null( await radioBlue.GetAttributeAsync( "checked" ) );

        await radioRed.ClickAsync();
        //TODO : FAILING; NO CHECKED Attribute
        //Assert.NotNull( await radioRed.GetAttributeAsync( "checked" ) );
        Assert.Null( await radioGreen.GetAttributeAsync( "checked" ) );
        Assert.Null( await radioBlue.GetAttributeAsync( "checked" ) );
        await Expect( result ).ToHaveTextAsync( "red" );


        await radioBlue.ClickAsync();
        //TODO : FAILING; NO CHECKED Attribute
        //Assert.NotNull( await radioBlue.GetAttributeAsync( "checked" ) );
        Assert.Null( await radioRed.GetAttributeAsync( "checked" ) );
        Assert.Null( await radioGreen.GetAttributeAsync( "checked" ) );
        await Expect( result ).ToHaveTextAsync( "blue" );
    }
}