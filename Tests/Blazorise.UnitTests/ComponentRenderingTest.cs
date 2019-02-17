using System;
using System.Collections.Generic;
using System.Text;
using DevHostServerProgram = Blazorise.BasicTestApp.Server.Program;
using Blazorise.UnitTests.Infrastructure;
using Blazorise.UnitTests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;
using Blazorise.BasicTestApp.Client;

namespace Blazorise.UnitTests
{
    public class ComponentRenderingTest : BasicTestAppTestBase
    {
        public ComponentRenderingTest(
               BrowserFixture browserFixture,
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
            Assert.Equal( "hello", btnElement.Text );
        }
    }
}
