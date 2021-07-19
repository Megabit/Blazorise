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
    public class TabsTest : BasicTestAppTestBase
    {
        public TabsTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<TabsComponent>();
        }

        [Fact]
        public void CanSelectTabs()
        {
            var paragraph = Browser.FindElement( By.Id( "basic-tabs" ) );
            var links = paragraph.FindElements( By.TagName( "a" ) );
            var content = paragraph.FindElement( By.ClassName( "tab-content" ) );
            var panels = content.FindElements( By.TagName( "div" ) );

            Assert.NotEmpty( links );
            Assert.NotEmpty( panels );

            WaitAssert.False( () => links[0].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.True( () => links[1].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => links[2].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => panels[0].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.True( () => panels[1].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => panels[2].GetAttribute( "class" ).Contains( "show" ) );

            links[0].Click();
            WaitAssert.True( () => links[0].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => links[1].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => links[2].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.True( () => panels[0].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => panels[1].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => panels[2].GetAttribute( "class" ).Contains( "show" ) );

            links[2].Click();
            WaitAssert.False( () => links[0].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => links[1].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.True( () => links[2].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => panels[0].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.False( () => panels[1].GetAttribute( "class" ).Contains( "show" ) );
            WaitAssert.True( () => panels[2].GetAttribute( "class" ).Contains( "show" ) );
        }
    }
}
