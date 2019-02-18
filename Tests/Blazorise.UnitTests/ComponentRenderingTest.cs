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

        [Fact]
        public void CanTextEditChangeText()
        {
            var appElement = MountTestComponent<TextEditComponent>();
            var txtElement = appElement.FindElement( By.TagName( "input" ) );

            txtElement.SendKeys( "abc" );
            WaitAssert.Contains( "abc", () => txtElement.GetAttribute( "value" ) );
        }

        [Fact]
        public void CanTextEditBindValue()
        {
            var appElement = MountTestComponent<TextEditComponent>();
            var txtElement = appElement.FindElement( By.TagName( "input" ) );
            var output = Browser.FindElement( By.Id( "test-result" ) );

            WaitAssert.Equal( string.Empty, () => output.Text );

            SendKeysSequentially( txtElement, "abcdefghijklmnopqrstuvwxyz" );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxyz", () => output.Text );

            txtElement.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxy", () => output.Text );
        }

        [Fact]
        public void CanValidateTextEditComponent()
        {
            var appElement = MountTestComponent<TextEditValidationComponent>();
            var txtElement = appElement.FindElement( By.TagName( "input" ) );

            Assert.NotNull( appElement.FindElement( By.ClassName( "invalid-feedback" ) ) );

            txtElement.SendKeys( "a" );
            WaitAssert.NotNull( () => appElement.FindElement( By.ClassName( "valid-feedback" ) ) );

            txtElement.SendKeys( Keys.Backspace );
            WaitAssert.NotNull( () => appElement.FindElement( By.ClassName( "invalid-feedback" ) ) );
        }

        void SendKeysSequentially( IWebElement target, string text )
        {
            // Calling it for each character works around some chars being skipped
            // https://stackoverflow.com/a/40986041
            foreach ( var c in text )
            {
                target.SendKeys( c.ToString() );
            }
        }
    }
}
