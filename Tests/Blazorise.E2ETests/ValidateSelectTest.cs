#region Using directives
using BasicTestApp.Client;
using Blazorise.E2ETests.Infrastructure;
using Blazorise.E2ETests.Infrastructure.ServerFixtures;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;
using DevHostServerProgram = BasicTestApp.Server.Program;
#endregion

namespace Blazorise.E2ETests
{
    public class ValidateSelectTest : BasicTestAppTestBase
    {
        public ValidateSelectTest( BrowserFixture browserFixture,
                  ToggleExecutionModeServerFixture<DevHostServerProgram> serverFixture,
                  ITestOutputHelper output )
                  : base( browserFixture, serverFixture, output )
        {
            Navigate( ServerPathBase, noReload: !serverFixture.UsingAspNetHost );
            MountTestComponent<ValidateSelectComponent>();
        }

        [Fact]
        public void CanValidateString_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-string-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateStringWithBind_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-string-with-bind-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateStringWithBind_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-string-with-bind-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateStringWithEvent_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-string-with-event-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateStringWithEvent_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-string-with-event-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateInt_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-int-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateIntWithBind_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-int-with-bind-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateIntWithBind_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-int-with-bind-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateEnum_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-enum-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateEnumWithBind_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-enum-with-bind-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateEnumWithBind_InitiallySelected()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-enum-with-bind-initially-selected" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }

        [Fact]
        public void CanValidateMultiString_InitiallyBlank()
        {
            var paragraph = Browser.FindElement( By.Id( "validate-multi-string-initially-blank" ) );
            var select = new SelectElement( paragraph.FindElement( By.TagName( "select" ) ) );

            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 1 );
            select.SelectByIndex( 2 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.DeselectByIndex( 1 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.DeselectByIndex( 2 );
            WaitAssert.True( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );

            select.SelectByIndex( 0 );
            WaitAssert.False( () => paragraph.ElementIsPresent( By.ClassName( "invalid-feedback" ) ) );
        }
    }
}
