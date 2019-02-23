#region Using directives
using BasicTestApp.Client;
using Blazorise.UnitTests.Infrastructure;
using Blazorise.UnitTests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.UnitTests
{
    public class ComponentRenderingTest : BasicTestAppTestBase
    {
        public ComponentRenderingTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
        }

        [Fact]
        public void BasicTestAppCanBeServed()
        {
            Assert.Equal( "Blazorise test app", Browser.Title );
        }

        [Fact]
        public void CanRenderTextOnlyComponent()
        {
            var appElement = MountTestComponent<TextOnlyComponent>();
            Assert.Equal( "Hello from TextOnlyComponent", appElement.Text );
        }

        [Fact]
        public void CanRenderButtonComponent()
        {
            var appElement = MountTestComponent<ButtonComponent>();
            var btnElement = appElement.FindElement( By.TagName( "button" ) );
            Assert.Equal( "hello primary", btnElement.Text );
            Assert.Equal( "btn btn-primary", btnElement.GetAttribute( "class" ) );
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
