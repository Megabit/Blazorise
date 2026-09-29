namespace Blazorise.E2E.Tests.Tests.Components.Steps;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class StepsTests : BlazorisePageTest
{
    [TestCase( "Enter" )]
    [TestCase( "Space" )]
    public async Task ActivationKey_ShouldSelectFocusedStep( string key )
    {
        await SelectKeyboardComponent();

        var group = Page.Locator( "#keyboard-steps-basic" );
        var steps = group.Locator( "[role=tab]" );
        var panels = group.Locator( "[role=tabpanel]" );

        await group.Locator( ".before-steps" ).FocusAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( steps.Nth( 1 ) ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "ArrowLeft" );
        await Expect( steps.First ).ToBeFocusedAsync();
        await Expect( steps.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );
        await Page.Keyboard.PressAsync( key );

        await Expect( steps.First ).ToHaveAttributeAsync( "aria-selected", "true" );
        await Expect( panels.First ).ToHaveAttributeAsync( "aria-hidden", "false" );
        await Expect( panels.Nth( 1 ) ).ToHaveAttributeAsync( "aria-hidden", "true" );
        await Expect( steps.First ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( panels.First ).ToBeFocusedAsync();
    }

    [Test]
    public async Task NavigationKeys_ShouldMoveFocusWithoutSelectingAndReturnToSelectedStep()
    {
        await SelectKeyboardComponent();

        var group = Page.Locator( "#keyboard-steps-basic" );
        var steps = group.Locator( "[role=tab]" );

        await steps.Nth( 1 ).FocusAsync();
        await Page.Keyboard.PressAsync( "End" );
        await Expect( steps.Last ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "ArrowRight" );
        await Expect( steps.First ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "ArrowLeft" );
        await Expect( steps.Last ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Home" );
        await Expect( steps.First ).ToBeFocusedAsync();
        await Expect( steps.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );

        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( group.Locator( "[role=tabpanel]" ).Nth( 1 ) ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( steps.Nth( 1 ) ).ToBeFocusedAsync();
    }

    [TestCase( "Enter" )]
    [TestCase( "Space" )]
    public async Task RejectedKeyboardNavigation_ShouldKeepSelectionAndFocus( string key )
    {
        await SelectKeyboardComponent();

        var group = Page.Locator( "#keyboard-steps-guarded" );
        var steps = group.Locator( "[role=tab]" );

        await steps.Nth( 1 ).FocusAsync();
        await Page.Keyboard.PressAsync( "ArrowRight" );
        await Page.Keyboard.PressAsync( key );

        await Expect( steps.Last ).ToBeFocusedAsync();
        await Expect( steps.Last ).ToHaveAttributeAsync( "aria-selected", "false" );
        await Expect( steps.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );
        await Expect( group.Locator( "[role=tabpanel]" ).Nth( 1 ) ).ToHaveAttributeAsync( "aria-hidden", "false" );
    }

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

    private async Task SelectKeyboardComponent()
    {
        await SelectTestComponent<StepsKeyboardComponent>();
        await Expect( Page.Locator( "#keyboard-steps-ready" ) ).ToHaveTextAsync( "Ready" );
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