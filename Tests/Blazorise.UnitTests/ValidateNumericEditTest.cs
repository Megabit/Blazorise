#region Using directives
using System.Diagnostics;
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
    public class ValidateNumericEditTest : BasicTestAppTestBase
    {
        public ValidateNumericEditTest( BrowserFixture browserFixture,
               ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
               ITestOutputHelper output )
               : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<ValidateNumericEditComponent>();
        }

        [Fact]
        public void CanValidateNumeric_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-numeric-initially-blank" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "1" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        // This test is little diferent because blazor is not working best with default values sent to component. see https://github.com/aspnet/AspNetCore/issues/7898
        // I will leave this test here for future reference in case the bug is fixed.
        [Fact]
        public void CanValidateNumeric_InitiallyPopulated()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-numeric-initially-populated" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "b" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateNumericWithBind_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-numeric-with-bind-initially-blank" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "1" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateNumericWithBind_InitiallyPopulated()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-numeric-with-bind-initially-populated" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "2" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateNumericWithEvent_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-numeric-with-event-initially-blank" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "1" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateNumericWithEvent_InitiallyPopulated()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-numeric-with-event-initially-populated" ) );
            var edit = paragraph.FindElement( By.TagName( "input" ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( Keys.Backspace );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            edit.SendKeys( "2" );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }
    }
}
