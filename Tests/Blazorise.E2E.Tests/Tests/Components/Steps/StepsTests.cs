namespace Blazorise.E2E.Tests.Tests.Components.Steps;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class StepsTests : BlazorisePageTest
{
    [Test]
    public async Task CanSelectSteps()
    {
        await SelectTestComponent<StepsComponent>();


        var sut = Page.Locator( "#basic-steps" );
        var links = await sut.Locator( "li" ).AllAsync();

        var tabContent = sut.Locator( ".steps-content" );
        var panels = await tabContent.Locator( "div" ).AllAsync();

        Assert.IsNotEmpty( links );
        Assert.IsNotEmpty( panels );

        await DoNotExpectActiveStepClass( links[0] );
        await ExpectActiveStepClass( links[1] );
        await DoNotExpectActiveStepClass( links[2] );


        await DoNotExpectActiveStepContentClass( panels[0] );
        await ExpectActiveStepContentClass( panels[1] );
        await DoNotExpectActiveStepContentClass( panels[2] );

        await links[0].ClickAsync();
        await ExpectActiveStepClass( links[0] );
        await DoNotExpectActiveStepClass( links[1] );
        await DoNotExpectActiveStepClass( links[2] );


        await ExpectActiveStepContentClass( panels[0] );
        await DoNotExpectActiveStepContentClass( panels[1] );
        await DoNotExpectActiveStepContentClass( panels[2] );

        await links[2].ClickAsync();
        await DoNotExpectActiveStepClass( links[0] );
        await DoNotExpectActiveStepClass( links[1] );
        await ExpectActiveStepClass( links[2] );


        await DoNotExpectActiveStepContentClass( panels[0] );
        await DoNotExpectActiveStepContentClass( panels[1] );
        await ExpectActiveStepContentClass( panels[2] );

    }

    private async Task ExpectActiveStepClass( ILocator locator )
    {
        await Expect( locator ).ToHaveClassAsync( expected: new Regex( "step step-active" ) );
    }

    private async Task DoNotExpectActiveStepClass( ILocator locator )
    {
        await Expect( locator ).Not.ToHaveClassAsync( expected: new Regex( "step step-active" ) );
    }

    private async Task ExpectActiveStepContentClass( ILocator locator )
    {
        await Expect( locator ).ToHaveClassAsync( expected: new Regex( "step-panel active" ) );
    }

    private async Task DoNotExpectActiveStepContentClass( ILocator locator )
    {
        await Expect( locator ).Not.ToHaveClassAsync( expected: new Regex( "step-panel active" ) );
    }

}