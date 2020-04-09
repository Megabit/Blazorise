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
            var appElement = MountTestComponent<ButtonOnlyComponent>();
            var btnElement = appElement.FindElement( By.TagName( "button" ) );
            Assert.Equal( "hello primary", btnElement.Text );
            Assert.Equal( "btn btn-primary", btnElement.GetAttribute( "class" ) );
        }

        [Fact]
        public void CannotChangeElementId()
        {
            var appElement = MountTestComponent<ElementIdComponent>();
            var date = appElement.FindElement( By.TagName( "input" ) );
            var button = appElement.FindElement( By.TagName( "button" ) );

            Assert.NotEqual( string.Empty, date.GetAttribute( "id" ) );

            var before = date.GetAttribute( "id" );

            button.Click();
            WaitAssert.Equal( before, () => date.GetAttribute( "id" ) );
        }
    }
}
