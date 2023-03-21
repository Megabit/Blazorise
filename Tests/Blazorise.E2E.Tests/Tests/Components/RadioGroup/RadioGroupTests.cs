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

        await Expect( radioGreen ).ToBeCheckedAsync();
        await Expect( radioRed ).Not.ToBeCheckedAsync();
        await Expect( radioBlue ).Not.ToBeCheckedAsync();
        await Expect( result ).ToHaveTextAsync( "green" );

        await radioRed.ClickAsync();
        await Expect( radioRed ).ToBeCheckedAsync();
        await Expect( radioGreen ).Not.ToBeCheckedAsync();
        await Expect( radioBlue ).Not.ToBeCheckedAsync();
        await Expect( result ).ToHaveTextAsync( "red" );


        await radioBlue.ClickAsync();
        await Expect( radioBlue ).ToBeCheckedAsync();
        await Expect( radioRed ).Not.ToBeCheckedAsync();
        await Expect( radioGreen ).Not.ToBeCheckedAsync();
        await Expect( result ).ToHaveTextAsync( "blue" );
    }
}