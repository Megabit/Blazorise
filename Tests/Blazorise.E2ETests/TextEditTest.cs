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
            var paragraph = Browser.FindElement( By.Id( "text-basic" ) );
            var text = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.Equal( string.Empty, () => text.GetAttribute( "value" ) );

            text.SendKeys( "abc" );
            WaitAssert.Contains( "abc", () => text.GetAttribute( "value" ) );
        }

        [Fact]
        public void CanChangeTextUsingEvent()
        {
            var paragraph = Browser.FindElement( By.Id( "text-event-initially-blank" ) );
            var text = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "text-event-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => result.Text );

            text.SendKeysSequentially( "abcdefghijklmnopqrstuvwxyz" );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxyz", () => result.Text );

            text.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxy", () => result.Text );
        }

        [Fact]
        public void CanChangeTextUsingBind()
        {
            var paragraph = Browser.FindElement( By.Id( "text-bind-initially-blank" ) );
            var text = paragraph.FindElement( By.TagName( "input" ) );
            var result = paragraph.FindElement( By.Id( "text-bind-initially-blank-result" ) );

            WaitAssert.Equal( string.Empty, () => result.Text );

            text.SendKeysSequentially( "abcdefghijklmnopqrstuvwxyz" );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxyz", () => result.Text );

            text.SendKeys( Keys.Backspace );
            WaitAssert.Equal( "abcdefghijklmnopqrstuvwxy", () => result.Text );
        }
    }
}
