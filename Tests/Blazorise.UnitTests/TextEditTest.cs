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
    public class TextEditTest : BasicTestAppTestBase
    {
        public TextEditTest( BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
            ITestOutputHelper output )
            : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<TextEditComponent>();
        }

        [Fact]
        public void CanChangeText()
        {
            var appElement = MountTestComponent<TextEditComponent>();
            var txtElement = appElement.FindElement( By.TagName( "input" ) );

            txtElement.SendKeys( "abc" );
            WaitAssert.Contains( "abc", () => txtElement.GetAttribute( "value" ) );
        }

        [Fact]
        public void CanBindText()
        {
            var appElement = MountTestComponent<TextEditComponent>();
            var txtElement = appElement.FindElement( By.TagName( "input" ) );
            var output = Browser.FindElement( By.Id( "test-result" ) );

            WaitAssert.Equal( string.Empty, () => output.Text );

            txtElement.SendKeysSequentially( "abcdefghijklmnopqrstuvwxyz" );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxyz", () => output.Text );

            txtElement.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxy", () => output.Text );
        }
    }
}
