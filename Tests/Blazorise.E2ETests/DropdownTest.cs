#region Using directives
using BasicTestApp.Client;
using Blazorise.UnitTests.Infrastructure;
using Blazorise.UnitTests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.UnitTests
{
    public class DropdownTest : BasicTestAppTestBase
    {
        public DropdownTest( BrowserFixture browserFixture,
               ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
               ITestOutputHelper output )
               : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<DropdownComponent>();
        }

        [Fact]
        public void CanShowAndHideDropdownComponent()
        {
            var appElement = MountTestComponent<DropdownComponent>();
            var drpElement = appElement.FindElement( By.ClassName( "dropdown" ) );
            var btnElement = drpElement.FindElement( By.TagName( "button" ) );
            var mnuElement = drpElement.FindElement( By.ClassName( "dropdown-menu" ) );

            btnElement.Click();
            WaitAssert.Contains( "show", () => drpElement.GetAttribute( "class" ) );
            WaitAssert.Contains( "show", () => mnuElement.GetAttribute( "class" ) );

            btnElement.Click();
            WaitAssert.NotContains( "show", () => drpElement.GetAttribute( "class" ) );
            WaitAssert.NotContains( "show", () => mnuElement.GetAttribute( "class" ) );
        }
    }
}
