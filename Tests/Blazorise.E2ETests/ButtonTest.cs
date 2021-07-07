#region Using directives
using BasicTestApp.Client;
using Blazorise.E2ETests.Infrastructure;
using Blazorise.E2ETests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.E2ETests
{
    public class ButtonTest : BasicTestAppTestBase
    {
        public ButtonTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<ButtonComponent>();
        }

        [Fact]
        public void CanRaiseCallback()
        {
            var paragraph = Browser.FindElement( By.Id( "basic-button-event" ) );
            var button = paragraph.FindElement( By.TagName( "button" ) );
            var result = paragraph.FindElement( By.Id( "basic-button-event-result" ) );

            WaitAssert.Equal( "0", () => result.Text );

            button.Click();
            WaitAssert.Equal( "1", () => result.Text );

            button.Click();
            WaitAssert.Equal( "2", () => result.Text );
        }
    }
}
