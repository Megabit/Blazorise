namespace Blazorise.E2E.Tests.Tests.Components.Tabs;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class TabsTests : BlazorisePageTest
{
    [TestCase( "Enter" )]
    [TestCase( "Space" )]
    public async Task ActivationKey_ShouldSelectFocusedTab( string key )
    {
        await SelectTestComponent<TabsKeyboardComponent>();

        ILocator group = Page.Locator( "#keyboard-tabs-Top" );
        ILocator firstTab = group.Locator( "[role=tab]" ).First;

        await group.Locator( "[role=tab]" ).Nth( 1 ).FocusAsync();
        await Page.Keyboard.PressAsync( "ArrowLeft" );
        await Expect( firstTab ).ToBeFocusedAsync();
        await Expect( firstTab ).ToHaveAttributeAsync( "aria-selected", "false" );
        await Page.Keyboard.PressAsync( key );

        await Expect( firstTab ).ToHaveAttributeAsync( "aria-selected", "true" );
        await Expect( firstTab ).ToHaveAttributeAsync( "tabindex", "0" );
        await Expect( group.Locator( "[role=tabpanel]" ).First ).ToBeVisibleAsync();
    }

    [TestCase( "Top", "ArrowRight", "ArrowLeft", "ArrowDown" )]
    [TestCase( "Start", "ArrowDown", "ArrowUp", "ArrowRight" )]
    public async Task ArrowKeys_ShouldFocusEnabledTabsAndWrapWithoutSelecting( string position, string nextKey, string previousKey, string unusedKey )
    {
        await SelectTestComponent<TabsKeyboardComponent>();

        ILocator group = Page.Locator( $"#keyboard-tabs-{position}" );
        ILocator tabs = group.Locator( "[role=tab]" );

        await group.Locator( ".before-tabs" ).FocusAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( tabs.Nth( 1 ) ).ToBeFocusedAsync();

        await Page.Keyboard.PressAsync( unusedKey );
        await Expect( tabs.Nth( 1 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );

        await Page.Keyboard.PressAsync( nextKey );
        await Expect( tabs.Nth( 3 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 3 ) ).ToHaveAttributeAsync( "aria-selected", "false" );
        await Expect( group.Locator( "[role=tabpanel]" ).Nth( 1 ) ).ToBeVisibleAsync();

        await Page.Keyboard.PressAsync( nextKey );
        await Expect( tabs.Nth( 0 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 0 ) ).ToHaveAttributeAsync( "aria-selected", "false" );

        await Page.Keyboard.PressAsync( previousKey );
        await Expect( tabs.Nth( 3 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 3 ) ).ToHaveAttributeAsync( "aria-selected", "false" );

        await Page.Keyboard.PressAsync( "Home" );
        await Expect( tabs.Nth( 0 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 0 ) ).ToHaveAttributeAsync( "aria-selected", "false" );

        await Page.Keyboard.PressAsync( "End" );
        await Expect( tabs.Nth( 3 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 3 ) ).ToHaveAttributeAsync( "aria-selected", "false" );
        await Expect( tabs.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );
    }

    [TestCase( "Top" )]
    [TestCase( "Start" )]
    public async Task TabKey_ShouldMoveBetweenActiveTabAndPanel( string position )
    {
        await SelectTestComponent<TabsKeyboardComponent>();

        ILocator group = Page.Locator( $"#keyboard-tabs-{position}" );
        ILocator activeTab = group.Locator( "[role=tab][aria-selected=true]" );
        ILocator activePanel = group.Locator( "[role=tabpanel][aria-hidden=false]" );

        await group.Locator( ".before-tabs" ).FocusAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( activeTab ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( activePanel ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( activeTab ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( group.Locator( ".after-tabs" ) ).ToBeFocusedAsync();
    }

    [TestCase( "Top", "ArrowLeft", 0 )]
    [TestCase( "Top", "ArrowRight", 3 )]
    [TestCase( "Start", "ArrowUp", 0 )]
    [TestCase( "Start", "ArrowDown", 3 )]
    public async Task TabKey_FromUnselectedTab_ShouldLeaveListAndReturnToSelectedTab( string position, string key, int focusedIndex )
    {
        await SelectTestComponent<TabsKeyboardComponent>();

        ILocator group = Page.Locator( $"#keyboard-tabs-{position}" );
        ILocator tabs = group.Locator( "[role=tab]" );

        await tabs.Nth( 1 ).FocusAsync();
        await Page.Keyboard.PressAsync( key );
        await Expect( tabs.Nth( focusedIndex ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( focusedIndex ) ).ToHaveAttributeAsync( "aria-selected", "false" );

        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( group.Locator( "[role=tabpanel]" ).Nth( 1 ) ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( tabs.Nth( 1 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );
    }

    [Test]
    public async Task NonFocusablePanel_ShouldSkipPanelAndKeepContentAndTabsAccessible()
    {
        await SelectTestComponent<TabsKeyboardComponent>();

        ILocator group = Page.Locator( "#keyboard-tabs-skip-panel" );
        ILocator tabs = group.Locator( "[role=tab]" );
        ILocator panels = group.Locator( "[role=tabpanel]" );

        await tabs.First.FocusAsync();
        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( panels.First.Locator( "button" ) ).ToBeFocusedAsync();
        await Expect( panels.First ).ToHaveAttributeAsync( "tabindex", "-1" );

        await Page.Keyboard.PressAsync( "Shift+Tab" );
        await Expect( tabs.First ).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync( "ArrowRight" );
        await Expect( tabs.Nth( 1 ) ).ToBeFocusedAsync();
        await Expect( tabs.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "false" );
        await Page.Keyboard.PressAsync( "Enter" );
        await Expect( tabs.Nth( 1 ) ).ToHaveAttributeAsync( "aria-selected", "true" );

        await Page.Keyboard.PressAsync( "Tab" );
        await Expect( panels.Nth( 1 ).Locator( "button" ) ).ToBeFocusedAsync();
        await Expect( panels.Nth( 1 ) ).ToHaveAttributeAsync( "tabindex", "-1" );
    }

    [Test]
    public async Task CanSelectTabs()
    {
        await SelectTestComponent<TabsComponent>();


        var sut = Page.Locator( "#basic-tabs" );
        var links = await sut.Locator( "a" ).AllAsync();

        var tabContent = sut.Locator( ".tab-content" );
        var panels = await tabContent.Locator( "div" ).AllAsync();

        Assert.IsNotEmpty( links );
        Assert.IsNotEmpty( panels );

        await DoNotExpectShowClass( links[0] );
        await ExpectShowClass( links[1] );
        await DoNotExpectShowClass( links[2] );


        await DoNotExpectShowClass( panels[0] );
        await ExpectShowClass( panels[1] );
        await DoNotExpectShowClass( panels[2] );

        await links[0].ClickAsync();
        await ExpectShowClass( links[0] );
        await DoNotExpectShowClass( links[1] );
        await DoNotExpectShowClass( links[2] );


        await ExpectShowClass( panels[0] );
        await DoNotExpectShowClass( panels[1] );
        await DoNotExpectShowClass( panels[2] );

        await links[2].ClickAsync();
        await DoNotExpectShowClass( links[0] );
        await DoNotExpectShowClass( links[1] );
        await ExpectShowClass( links[2] );


        await DoNotExpectShowClass( panels[0] );
        await DoNotExpectShowClass( panels[1] );
        await ExpectShowClass( panels[2] );

    }

    private async Task ExpectShowClass( ILocator locator )
    {
        await Expect( locator ).ToHaveClassAsync( expected: new Regex( "show" ) );
    }

    private async Task DoNotExpectShowClass( ILocator locator )
    {
        await Expect( locator ).Not.ToHaveClassAsync( expected: new Regex( "show" ) );
    }

}