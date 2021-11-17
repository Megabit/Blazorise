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
    public class AsyncValidateTextEditTest : BasicTestAppTestBase
    {
        public AsyncValidateTextEditTest( BrowserFixture browserFixture,
               ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
               ITestOutputHelper output )
               : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<AsyncValidateTextEditComponent>();
        }

        [Fact]
        public void CanValidateText_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-text-initially-blank" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "a" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateText_InitiallyPopulated()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-text-initially-populated" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "b" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateTextWithBind_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-text-with-bind-initially-blank" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "a" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateTextWithBind_InitiallyPopulated()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-text-with-bind-initially-populated" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "b" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateTextWithEvent_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-text-with-event-initially-blank" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "a" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateTextWithEvent_InitiallyPopulated()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-text-with-event-initially-populated" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "b" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }
    }
}
